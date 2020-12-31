using System;

using Shared.Json;
using Shared.Xml;

namespace Server {
    class Program {
        static void Main( string[ ] args ) {

            //var server = new XmlSocketServer();
            var server = new JsonSocketServer();

            server.Bind<MessageHandler>( );
            server.Start( );

            Console.WriteLine( "Echo Server running" );
            Console.ReadLine( );
        }
    }
}
