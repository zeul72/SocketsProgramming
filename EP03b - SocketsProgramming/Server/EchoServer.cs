#nullable enable

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

using Shared;
using Shared.Xml;

namespace Server
{

    //NOTES: 
    //1. Update XmlMessageProtocol to special case handling of XDocument EncodeBody<T>
    //2. 


    public class EchoServer
    {

        readonly XDocumentMessageDispatcher                     _messageDispatcher = new XDocumentMessageDispatcher();

        public EchoServer( )
        {
            _messageDispatcher.Register<HeartBeatRequestMessage, HeartBeatResponseMessage>( MessageHandler.HeartBeatRequestHandler );
        }

        public void Start( int port = 9000 )
        {
            var endPoint = new IPEndPoint( IPAddress.Loopback, port);

            var socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp );
            socket.Bind( endPoint );
            socket.Listen( 128 );

            _ = Task.Run( async ( ) => {
                do
                {
                    var clientSocket = await Task.Factory.FromAsync(
                    new Func<AsyncCallback, object?, IAsyncResult>(socket.BeginAccept),
                    new Func<IAsyncResult, Socket>(socket.EndAccept),
                    null).ConfigureAwait(false);

                    var channel = new XmlChannel();

                    channel.OnMessage( async message => {
                        var response = await _messageDispatcher.DispatchAsync(message).ConfigureAwait(false);
                        if ( response != null )
                            await channel.SendAsync( response ).ConfigureAwait( false );
                    } );

                    try
                    {
                        channel.Attach( clientSocket );
                        while ( true ) { }
                    } catch
                    {
                        channel.Dispose( );
                    }

                } while ( true );
            } );

        }
    }
}
