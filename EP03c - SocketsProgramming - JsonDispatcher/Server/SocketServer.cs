using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

using Shared;

namespace Server
{


    public class SocketServer
    {

        // readonly XDocumentMessageDispatcher _messageDispatcher = new XDocumentMessageDispatcher();
        readonly JsonMessageDispatcher _messageDispatcher = new JsonMessageDispatcher();

        public SocketServer( ) => _messageDispatcher.BindController<MessageHandler>( );


        public void Start( int port = 9000 )
        {
            var endPoint = new IPEndPoint( IPAddress.Loopback, port);

            var socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp );
            socket.Bind( endPoint );
            socket.Listen( 128 );

            _ = Task.Run( ( ) => HandleConnections( socket ) );

        }

        private async Task HandleConnections( Socket socket )
        {

            do {
                var clientSocket = await Task.Factory.FromAsync(
                    new Func<AsyncCallback, object, IAsyncResult>(socket.BeginAccept),
                    new Func<IAsyncResult, Socket>(socket.EndAccept),
                    null).ConfigureAwait(false);

                Console.WriteLine( "SocketServer :: CLIENT CONNECTED" );

                //var channel = new XmlChannel();
                var channel = new JsonChannel();
                _messageDispatcher.BindChannel( channel );
                
                channel.Attach( clientSocket );

                while ( true ) { }

            } while ( true );
        }

    }
}
