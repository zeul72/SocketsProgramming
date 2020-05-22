using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Client {

    public class MyMessage {
        public string StringProperty { get; set; }
        public int IntProperty { get; set; }
    }



    class Program {

        static async Task SendAsync<T>( NetworkStream networkStream, T message ) {
            var (header, body) = Encode( message );
            await networkStream.WriteAsync( header, 0, header.Length ).ConfigureAwait( false );
            await networkStream.WriteAsync( body, 0, body.Length ).ConfigureAwait( false );
        }

        static async Task<T> ReceiveAsync<T>( NetworkStream networkStream ) {

            var headerBytes = await ReadAsync(networkStream, 4);
            var bodyLength = IPAddress.NetworkToHostOrder( BitConverter.ToInt32(headerBytes) );

            var bodyBytes = await ReadAsync(networkStream, bodyLength);

            return Decode<T>( bodyBytes );
        }

        static (byte[ ] header, byte[ ] body) Encode<T>( T message ) {
            var xs = new XmlSerializer(typeof(T));
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            xs.Serialize( sw, message );

            var bodyBytes = Encoding.UTF8.GetBytes(sb.ToString());
            var headerBytes = BitConverter.GetBytes( IPAddress.HostToNetworkOrder( bodyBytes.Length ));

            return (headerBytes, bodyBytes);
        }

        static T Decode<T>( byte[ ] body ) {
            var str = Encoding.UTF8.GetString(body);
            var sr = new StringReader(str);
            var xs = new XmlSerializer(typeof(T));
            return ( T )xs.Deserialize( sr );
        }

        static async Task<byte[ ]> ReadAsync( NetworkStream networkStream, int bytesToRead ) {
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



        static async Task Main( string[ ] args ) {

            Console.WriteLine( "Press Enter to Connect" );
            Console.ReadLine( );


            var endpoint = new IPEndPoint(IPAddress.Loopback, 9000);
            var socket = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect( endpoint );
            var networkStream = new NetworkStream(socket, true);

            var myMessage = new MyMessage {
                IntProperty = 404,
                StringProperty = "Hello World"
            };

            Console.WriteLine( "Sending" );
            Print( myMessage );

            await SendAsync( networkStream, myMessage ).ConfigureAwait( false );

            var responseMsg = await ReceiveAsync<MyMessage>(networkStream).ConfigureAwait(false);

            Console.WriteLine( "Received" );
            Print( responseMsg );


            Console.ReadLine( );

        }


        static void Print( MyMessage m ) => Console.WriteLine( $"MyMessage.IntProperty = {m.IntProperty}, MyMessage.StringProperty = {m.StringProperty}" );
    }
}
