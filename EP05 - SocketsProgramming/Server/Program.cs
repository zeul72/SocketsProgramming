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
            var posController = new POSController();
            var txController = new TransactionManager { POSController = posController };

            MessageHandler.POSController = posController;
            MessageHandler.TransactionManager = txController;


            var cancellationTokenSource = new CancellationTokenSource();

            //var server = new XmlSocketServer();
            var server = new JsonSocketServer( );

            server.Bind<MessageHandler>( );
            var serverTask = server.StartAsync(9000, cancellationTokenSource.Token);


            do {

                Console.WriteLine( "Echo Server running" );
                Console.WriteLine( "Press P to 'Pay' For Basket" );
                Console.WriteLine( "Press X to exit" );

                var key = Console.ReadKey();

                switch ( key.Key ) {
                    case ConsoleKey.X:
                        cancellationTokenSource.Cancel( );
                        await serverTask;
                        Console.WriteLine( "Server Shutdown" );
                        return;

                    case ConsoleKey.P:
                        await txController.PayBasket( ).ConfigureAwait( false );
                        break;
                }
            } while ( true );
        }
    }
}
