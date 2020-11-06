using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

using Shared;
using Shared.Xml;

namespace Server {


    public class EchoServer {

        readonly XDocumentMessageDispatcher _messageDispatcher = new XDocumentMessageDispatcher();

        public EchoServer()
        {
            _messageDispatcher.Register<HeartBeatRequestMessage, HeartBeatResponseMessage>( MessageHandler.HandleMessage );
            _messageDispatcher.Register<SubmitBasketRequest, SubmitBasketResponse>( MessageHandler.HandleMessage );
        }


        public void Start( int port = 9000 ) {
            var endPoint = new IPEndPoint( IPAddress.Loopback, port);

            var socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp );
            socket.Bind( endPoint );
            socket.Listen( 128 );

            _ = Task.Run( ( ) => DoEcho( socket ) );

        }

        private async Task DoEcho( Socket socket ) {

            do {
                var clientSocket = await Task.Factory.FromAsync(
                    new Func<AsyncCallback, object, IAsyncResult>(socket.BeginAccept),
                    new Func<IAsyncResult, Socket>(socket.EndAccept),
                    null).ConfigureAwait(false);

                Console.WriteLine( "ECHO SERVER :: CLIENT CONNECTED" );

                var channel = new XmlChannel();
                channel.OnMessage( async m => {
                    var response = await _messageDispatcher.DispatchAsync(m).ConfigureAwait(false);
                    if ( response != null )
                    {
                        try
                        {
                            await channel.SendAsync( response ).ConfigureAwait( false );
                        } catch(Exception _e)
                        {
                            Console.WriteLine( $"Oh NO!!! {_e}" );
                        }
                    }
                } );

                channel.Attach( clientSocket );

                while ( true ) { }

            } while( true );
        }

    }
}
