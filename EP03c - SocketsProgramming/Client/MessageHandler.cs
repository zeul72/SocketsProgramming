using System;
using System.Threading.Tasks;

using Shared;
using Shared.Json;
using Shared.Xml;

namespace Client
{
    public class MessageHandler
    {
        //Handler on the 'Client' side of the system
        [XPathRoute( "/Message[@type='Response' and @action='HeartBeat']" )]
        [JsonRoute( "$.action","HeartBeat" )]
        public static Task HandleMessage( HeartBeatResponseMessage response )
        {
            Console.WriteLine( $"Received HeartBeatResponseMessage Response: {response?.Result?.Status}, {response?.Id}" );
            return Task.CompletedTask;
        }

        [XPathRoute( "/Message[@type='Response' and @action='SubmitBasket']" )]
        [JsonRoute( "$.action", "SubmitBasket" )]
        public static Task HandleMessage( SubmitBasketResponse response )
        {
            Console.WriteLine( $"Received SubmitBasketResponse Response: {response?.Result?.Status}, {response?.Id}" );
            return Task.CompletedTask;
        }
    }
}
