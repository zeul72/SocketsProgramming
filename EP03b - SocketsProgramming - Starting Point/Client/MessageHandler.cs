using System;
using System.Threading.Tasks;

using Shared;

namespace Client
{
    public class MessageHandler
    {

        [Route( "/Message[@type='Response' and @action='HeartBeat']" )]
        public static Task HeartBeatResponseHandler( HeartBeatResponseMessage response )
        {
            Console.WriteLine( $"Received Response: {response?.Result?.Status}, {response?.Id}" );
            return Task.CompletedTask;
        }

    }
}
