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
        [JsonRoute( "$.action", "HeartBeat" )]
        public static Task HandleMessage( HeartBeatResponseMessage response )
        {
            Received( response );
            return Task.CompletedTask;
        }

        [XPathRoute( "/Message[@type='Response' and @action='SubmitBasket']" )]
        [JsonRoute( "$.action", "SubmitBasket" )]
        public static Task HandleMessage( SubmitBasketResponse response )
        {
            Received( response );
            return Task.CompletedTask;
        }


        [XPathRoute( "/Message[@type='Response' and @action='SubmitBasket']" )]
        [JsonRoute( "$.action", "BasketPaid" )]
        public static Task HandleMessage( BasketPaidRequest request )
        {
            Received( request );

            Console.WriteLine( "==========================[ Basket Paid ]==================================" );
            Console.WriteLine( $"Transaction: {request.POSTransactionNumber}" );
            Console.WriteLine( $"     Amount: {request.PaymentInfo.Amount:C}" );
            Console.WriteLine( $"       Card: ************{request.PaymentInfo.LastFour}" );

            return Task.CompletedTask;
        }


        static void Received<T>( IChannel channel, T msg ) where T : Message
       => Console.WriteLine( $"Received {typeof( T ).Name} From Channel {channel.Id}: POS ID [ {msg.POSData.Id} ], Action[ {msg.Action} ], Id[ {msg.Id} ]" );

        static void Received<T>( T msg ) where T : Message
            => Console.WriteLine( $"Received {typeof( T ).Name}: POS ID [ {msg.POSData.Id} ], Action[ {msg.Action} ], Id[ {msg.Id} ]" );

        static void Sending<T>( T msg ) where T : Message
            => Console.WriteLine( $"Sending {typeof( T ).Name}: POS ID [ {msg.POSData.Id} ], Action[ {msg.Action} ], Id[ {msg.Id} ]" );
    }
}
