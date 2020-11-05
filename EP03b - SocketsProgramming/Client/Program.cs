using System;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;

using Shared;
using Shared.Xml;

namespace Client
{

    class Program
    {

        static async Task Main( string[ ] args )
        {


            Console.WriteLine( "Press Enter to Connect" );
            Console.ReadLine( );

            var msgDispatcher = new XDocumentMessageDispatcher();
            msgDispatcher.Register<HeartBeatResponseMessage>( MessageHandler.HeartBeatResponseHandler );

            var endpoint = new IPEndPoint(IPAddress.Loopback, 9000);

            //var channel = new ClientChannel<JsonMessageProtocol,JObject>();
            var channel = new ClientChannel<XmlMessageProtocol,XDocument>();
            channel.OnMessage( msgDispatcher.DispatchAsync );

            await channel.ConnectAsync( endpoint ).ConfigureAwait( false );

            for ( int i = 1; ; i++ )
            {
                var hbRequest = new HeartBeatRequestMessage {
                    Id = $"HB{i}",
                    POSData = new POSData { Id="POS001"},
                };

                Console.WriteLine( "Sending" );
                await channel.SendAsync( hbRequest ).ConfigureAwait( false );
                await Task.Delay( 10 * 1000 );
            };
        }
    }
}
