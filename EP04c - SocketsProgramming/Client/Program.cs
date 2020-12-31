using System;
using System.Net;
using System.Threading.Tasks;

using Shared;
using Shared.Json;

namespace Client
{


    class Program
    {

        static int POSId = System.Diagnostics.Process.GetCurrentProcess().Id;

        //static XmlClientChannel Channel = new XmlClientChannel();
        static JsonClientChannel Channel = new JsonClientChannel();

        static async Task Main( string[ ] args )
        {

            Console.WriteLine( "Press Enter to Connect" );
            Console.ReadLine( );

            //var messageDispatcher = new XDocumentMessageDispatcher();
            var messageDispatcher = new JsonMessageDispatcher();

            messageDispatcher.Bind<MessageHandler>( );

            try {
                var endpoint = new IPEndPoint(IPAddress.Loopback, 9000);
                messageDispatcher.Bind( Channel );
                await Channel.ConnectAsync( endpoint ).ConfigureAwait( false );
                Console.WriteLine( "Client Running" );
                _ = Task.Run( ( ) => HBLoop( -1 ) );
            } catch(Exception _e) {
                Console.WriteLine( $"Client Exception: {_e}" );
            }
            Console.ReadLine( );
        }

        static async Task HBLoop( int count )
        {
            bool loopControl( int count )
                => count == -1 ? true : count-- > 0;

            try {
                while ( loopControl( count ) ) {
                    var hbMessage = new HeartBeatRequestMessage {
                        Id = "♥♥HB♥♥",
                        POSData = new POSData{ Id = $"POS{POSId}" }
                    };
                    await Channel.SendAsync( hbMessage ).ConfigureAwait( false );
                    await Task.Delay( 10 * 1000 );
                }
            } catch(Exception _e) {
                Console.WriteLine( $"Exception in HBLoop : {_e}" );
            }
        }
    }
}
