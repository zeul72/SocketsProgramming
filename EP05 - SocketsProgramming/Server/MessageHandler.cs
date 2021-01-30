using System;
using System.Threading.Tasks;

using Shared;
using Shared.Json;
using Shared.Xml;

namespace Server
{
    public class MessageHandler
    {
        //should be IoC/DI 
        public static POSController POSController { get; set; }
        public static TransactionManager TransactionManager { get; set; }

        //Handler on the 'Server' side of the system
        [XPathRoute( "/Message[@type='Request' and @action='HeartBeat']" )]
        [JsonRoute( "$.action", "HeartBeat" )]
        public static Task<HeartBeatResponseMessage> HandleMessage(IChannel channel, HeartBeatRequestMessage request )
        {

            Received( channel, request );

            POSController.ProcessHeartBeat( request.POSData.Id, channel );


            var response = new HeartBeatResponseMessage {
                Id = request.Id,
                POSData = request.POSData,
                Result = new Result { Status = Status.Success}
            };
            Sending( response );
            return Task.FromResult( response );
        }

        //Handler on the 'Server' side of the system
        [XPathRoute( "/Message[@type='Request' and @action='SubmitBasket']" )]
        [JsonRoute( "$.action", "SubmitBasket" )]
        public static Task<SubmitBasketResponse> HandleMessage( SubmitBasketRequest request )
        {
            Received( request );

            TransactionManager.ProcessBasket( request );

            var response = new SubmitBasketResponse {
                Id = request.Id,
                POSData = request.POSData,
                Result = new Result { Status = Status.Success}
            };
            Sending( response );
            return Task.FromResult( response );
        }

        static void Received<T>( IChannel channel, T msg ) where T : Message
            => Console.WriteLine( $"Received {typeof( T ).Name} From Channel {channel.Id}: POS ID [ {msg.POSData.Id} ], Action[ {msg.Action} ], Id[ {msg.Id} ]" );

        static void Received<T>( T msg ) where T : Message
            => Console.WriteLine( $"Received {typeof( T ).Name}: POS ID [ {msg.POSData.Id} ], Action[ {msg.Action} ], Id[ {msg.Id} ]" );

        static void Sending<T>( T msg ) where T : Message
            => Console.WriteLine( $"Sending {typeof( T ).Name}: POS ID [ {msg.POSData.Id} ], Action[ {msg.Action} ], Id[ {msg.Id} ]" );
    }
}
