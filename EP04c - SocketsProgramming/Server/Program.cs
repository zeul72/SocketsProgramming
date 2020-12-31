using System;
using System.Threading;
using System.Threading.Tasks;

using Shared.Json;

namespace Server
{
    class Program
    {
        static async Task Main( string[ ] args )
        {
            var cancellationTokenSource = new CancellationTokenSource();

            //var server = new XmlSocketServer();
            var server = new JsonSocketServer(2);

            server.Bind<MessageHandler>( );
            var serverTask = server.StartAsync(9000, cancellationTokenSource.Token);


            do {
                Console.WriteLine( "Echo Server running. Press X to exit" );
                var key = Console.ReadKey();
                if ( key.Key == ConsoleKey.X ) {
                    cancellationTokenSource.Cancel( );
                    await serverTask;
                    Console.WriteLine( "Server Shutdown" );
                    break;
                }
            } while ( true );
        }
    }
}
