using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Shared
{
    public abstract class SocketServer<TChannelType, TProtocol, TMessageType, TMessageDispatcher>
        where TChannelType : Channel<TProtocol, TMessageType>, new()
        where TProtocol : Protocol<TMessageType>, new()
        where TMessageType : class, new()
        where TMessageDispatcher : MessageDispatcher<TMessageType>, new()
    {
        Func<Socket> _serverSocketFactory;

        readonly ChannelManager        _channelManager;
        readonly TMessageDispatcher    _messageDispatcher = new TMessageDispatcher();

        readonly SemaphoreSlim              _connectionLimiter;

        public SocketServer( int maxConnections = 100_000_000 )
        {
            _connectionLimiter = new SemaphoreSlim( maxConnections, maxConnections );

            _channelManager = new ChannelManager( ( ) => {
                var channel = CreateChannel ( );
                _messageDispatcher.Bind( channel );
                return channel;
            } );


            _channelManager.ChannelClosed += ( s, e ) => _connectionLimiter.Release( );

        }

        public void Bind<TController>( )
         => _messageDispatcher.Bind<TController>( );

        public Task StartAsync( int port, CancellationToken cancellationToken )
        {
            _serverSocketFactory = ( ) => {
                var endPoint = new IPEndPoint( IPAddress.Loopback, port);
                var socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp );
                socket.Bind( endPoint );
                socket.Listen( 128 );
                return socket;
            };

            return Task.Factory.StartNew( ( ) => RunAsync( cancellationToken ), cancellationToken );
        }

        private async Task RunAsync( CancellationToken cancellationToken )
        {
            try {
                Socket serverSocket = null;

                do {

                    if ( !await _connectionLimiter.WaitAsync( 1000, cancellationToken ) ) {
                        Console.WriteLine( "SocketServer :: Max Connections Reached" );
                        //max connections reached, so nuke the server socket
                        try {
                            serverSocket?.Close( );
                            serverSocket?.Dispose( );
                            serverSocket = null;
                        } catch { }

                        await _connectionLimiter.WaitAsync( cancellationToken );
                    }

                    if ( !cancellationToken.IsCancellationRequested ) {
                        serverSocket ??= _serverSocketFactory( );
                        await AcceptConnection( serverSocket );
                    }

                } while ( !cancellationToken.IsCancellationRequested );

            } catch ( OperationCanceledException ) {
                //Expected exception for task cancellation - swallow it
            } catch ( Exception _e ) {
                //TODO: Log it somewhere good :)
                Console.WriteLine( $"Exception in SocketServer::RunAsync => {_e}" );
            }
        }

        async Task AcceptConnection( Socket socket )
        {
            var clientSocket = await Task.Factory.FromAsync( new Func<AsyncCallback, object, IAsyncResult>(socket.BeginAccept),
                                                             new Func<IAsyncResult, Socket>(socket.EndAccept),
                                                             null ).ConfigureAwait(false);

            Console.WriteLine( "SERVER :: CLIENT CONNECTION REQUEST" );
            _channelManager.Accept( clientSocket );
            Console.WriteLine( $"SERVER :: CLIENT CONNECTED :: {_connectionLimiter.CurrentCount}" );
        }

        protected virtual TChannelType CreateChannel( ) => new TChannelType( );

    }
}
