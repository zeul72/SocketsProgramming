using System;

namespace Server {
    class Program {
        static void Main( string[ ] args ) {

            var server = new SocketServer();

            server.Start( );

            Console.WriteLine( "Echo Server running" );
            Console.ReadLine( );
        }
    }
}
