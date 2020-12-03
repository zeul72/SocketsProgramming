using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Shared {

    public abstract class Channel<TProtocol, TMessageType> : IDisposable, IChannel where TProtocol : Protocol<TMessageType>, new()
    {

        protected bool _isDisposed = false;

        readonly TProtocol               _protocol                  = new TProtocol();
        readonly CancellationTokenSource _cancellationTokenSource   = new CancellationTokenSource();

        Func<TMessageType,Task> _messageCallback;
        NetworkStream           _networkStream;
        Task                    _receiveLoopTask;

        public Guid Id => Guid.NewGuid( );

        public void Attach( Socket socket )
        {
            _networkStream = new NetworkStream( socket, true );
            _receiveLoopTask = Task.Run( ReceiveLoop, _cancellationTokenSource.Token );
        }

        public void OnMessage( Func<TMessageType, Task> callbackHandler )
            => _messageCallback = callbackHandler;

        public void Close( )
        {
            _cancellationTokenSource.Cancel( );
            _networkStream?.Close( );
        }

        public async Task SendAsync<T>( T message )
            => await _protocol.SendAsync( _networkStream, message ).ConfigureAwait( false );

        protected virtual async Task ReceiveLoop( )
        {
            while ( !_cancellationTokenSource.Token.IsCancellationRequested ) {
                //TODO: Pass Cancellation Token to Protocol methods
                var msg = await _protocol.ReceiveAsync(_networkStream).ConfigureAwait(false);
                await _messageCallback( msg ).ConfigureAwait( false );
            }
        }

        ~Channel( ) => Dispose( false );
        public void Dispose( ) => Dispose( true );
        protected void Dispose( bool isDisposing )
        {
            if ( !_isDisposed ) {
                _isDisposed = true;

                Close( );
                //TODO: Clean up socket, stream, etc.
                _networkStream?.Dispose( );

                if ( isDisposing )
                    GC.SuppressFinalize( this );
            }
        }
    }
}
