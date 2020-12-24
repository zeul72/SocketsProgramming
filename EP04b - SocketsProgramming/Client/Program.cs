using System;
using System.Net;
using System.Threading.Tasks;

using Shared;
using Shared.Json;
using Shared.Xml;

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


            var endpoint = new IPEndPoint(IPAddress.Loopback, 9000);
            messageDispatcher.Bind( Channel );
            await Channel.ConnectAsync( endpoint ).ConfigureAwait( false );

            //_ = Task.Run( ( ) => HBLoop( 1 ) );

            var basket = new SubmitBasketRequest {
                Id = "TXN0007",
                POSData = new POSData{ Id = $"POS{POSId}"}
            };
            await Channel.SendAsync( basket ).ConfigureAwait( false );

            await Task.Delay( 1000 );

            Channel.Close( );
            Console.WriteLine( "Client Closed Channel" );

            Console.ReadLine( );
        }

        static async Task HBLoop( int count )
        {
            while ( count-- > 0 ) {
                var hbMessage = new HeartBeatRequestMessage {
                    Id = "♥♥HB♥♥",
                    POSData = new POSData{ Id = $"POS{POSId}" }
                };
                await Channel.SendAsync( hbMessage ).ConfigureAwait( false );
                await Task.Delay( 10 * 1000 );
            }
        }
    }
}
