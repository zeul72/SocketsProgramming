using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

using Shared;
using Shared.Json;
using Shared.Xml;

namespace Server
{


    public class SocketServer
    {

         readonly XDocumentMessageDispatcher _messageDispatcher = new XDocumentMessageDispatcher();
        //readonly JsonMessageDispatcher _messageDispatcher = new JsonMessageDispatcher();

        public SocketServer( )
        {
            //_messageDispatcher.Register<HeartBeatRequestMessage, HeartBeatResponseMessage>( MessageHandler.HandleMessage );
            //_messageDispatcher.Register<SubmitBasketRequest, SubmitBasketResponse>( MessageHandler.HandleMessage );
            _messageDispatcher.Bind<MessageHandler>( );
        }


        public void Start( int port = 9000 )
        {
            var endPoint = new IPEndPoint( IPAddress.Loopback, port);

            var socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp );
            socket.Bind( endPoint );
            socket.Listen( 128 );

            _ = Task.Run( ( ) => DoEcho( socket ) );

        }

        private async Task DoEcho( Socket socket )
        {

            do {
                var clientSocket = await Task.Factory.FromAsync(
                    new Func<AsyncCallback, object, IAsyncResult>(socket.BeginAccept),
                    new Func<IAsyncResult, Socket>(socket.EndAccept),
                    null).ConfigureAwait(false);

                Console.WriteLine( "ECHO SERVER :: CLIENT CONNECTED" );

                var channel = new XmlChannel();
                //var channel = new JsonChannel();
                _messageDispatcher.Bind( channel );

                channel.Attach( clientSocket );

                while ( true ) { }

            } while ( true );
        }

    }
}
