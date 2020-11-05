using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Shared {

    public abstract class Protocol<TMessageType> {

        const int HEADER_SIZE = 4;

        public async Task<TMessageType> ReceiveAsync( NetworkStream networkStream ) {
            var bodyLength = await ReadHeader(networkStream).ConfigureAwait(false);
            //TODO: Assert valid body length
            AssertValidMessageLength( bodyLength );
            return await ReadBody( networkStream, bodyLength ).ConfigureAwait( false );
        }

        public async Task SendAsync<T>( NetworkStream networkStream, T message ) {
            var (header, body) = Encode<T>( message );
            await networkStream.WriteAsync( header, 0, header.Length );
            await networkStream.WriteAsync( body, 0, body.Length );
        }

        async Task<int> ReadHeader( NetworkStream networkStream ) {
            var headerBytes = await ReadAsync(networkStream, HEADER_SIZE).ConfigureAwait(false);
            return IPAddress.NetworkToHostOrder( BitConverter.ToInt32( headerBytes ) );
        }

        async Task<TMessageType> ReadBody( NetworkStream networkStream, int bodyLength ) {
            var bodyBytes = await ReadAsync(networkStream, bodyLength).ConfigureAwait(false);
            return Decode( bodyBytes );
        }

        async Task<byte[ ]> ReadAsync( NetworkStream networkStream, int bytesToRead ) {
            var buffer = new byte[bytesToRead];
            var bytesRead = 0;
            while( bytesRead < bytesToRead ) {
                var bytesReceived = await networkStream.ReadAsync( buffer, bytesRead, ( bytesToRead - bytesRead ) ).ConfigureAwait( false );
                if( bytesReceived == 0 )
                    throw new Exception( "Socket Closed" );
                bytesRead += bytesReceived;
            }
            return buffer;
        }

        protected (byte[ ] header, byte[ ] body) Encode<T>( T message ) {
            var bodyBytes = EncodeBody<T>(message);
            var headerBytes = BitConverter.GetBytes( IPAddress.HostToNetworkOrder( bodyBytes.Length ));
            return (headerBytes, bodyBytes);
        }

        protected abstract TMessageType Decode( byte[ ] message );
      
        protected abstract byte[ ] EncodeBody<T>( T message );

        protected virtual void AssertValidMessageLength( int messageLength ) {
            if( messageLength < 1 )
                throw new ArgumentOutOfRangeException( "Invalid Message Length" );
        }
    }
}
