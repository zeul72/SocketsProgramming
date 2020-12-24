using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Shared
{
    public abstract class SocketServer<TChannelType, TProtocol, TMessageType, TMessageDispatcher>
        where TChannelType : Channel<TProtocol, TMessageType>, new()
        where TProtocol : Protocol<TMessageType>, new()
        where TMessageType : class, new()
        where TMessageDispatcher : MessageDispatcher<TMessageType>, new()
    {
        readonly ChannelManager        _channelManager;
        readonly TMessageDispatcher    _messageDispatcher = new TMessageDispatcher();

        public SocketServer( )
        {
            _channelManager = new ChannelManager( ( ) => {
                var channel = CreateChannel ( );
                _messageDispatcher.Bind( channel );
                return channel;
            } );
        }

        public void Bind<TController>( )
         => _messageDispatcher.Bind<TController>( );

        public void Start( int port = 9000 )
        {
            var endPoint = new IPEndPoint( IPAddress.Loopback, port);

            var socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp );
            socket.Bind( endPoint );
            socket.Listen( 128 );

            _ = Task.Run( ( ) => RunAsync( socket ) );

        }

        private async Task RunAsync( Socket socket )
        {

            do {
                var clientSocket = await Task.Factory.FromAsync(
                    new Func<AsyncCallback, object, IAsyncResult>(socket.BeginAccept),
                    new Func<IAsyncResult, Socket>(socket.EndAccept),
                    null).ConfigureAwait(false);

                Console.WriteLine( "SERVER :: CLIENT CONNECTION REQUEST" );

                _channelManager.Accept( clientSocket );

                Console.WriteLine( "SERVER :: CLIENT CONNECTED" );

            } while ( true );
        }

        
        protected virtual TChannelType CreateChannel( ) => new TChannelType( );

    }
}
