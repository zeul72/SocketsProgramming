using System;
using System.Threading.Tasks;

using Shared;

namespace Client
{
    public static class MessageHandler
    {
        //Handler on the 'Client' side of the system
        [Route( "/Message[@type='Response' and @action='HeartBeat']" )]
        public static Task HandleMessage( HeartBeatResponseMessage response )
        {
            Console.WriteLine( $"Received HeartBeatResponseMessage Response: {response?.Result?.Status}, {response?.Id}" );
            return Task.CompletedTask;
        }

        [Route( "/Message[@type='Response' and @action='SubmitBasket']" )]
        public static Task HandleMessage( SubmitBasketResponse response )
        {
            Console.WriteLine( $"Received SubmitBasketResponse Response: {response?.Result?.Status}, {response?.Id}" );
            return Task.CompletedTask;
        }
    }
}
