using System;
using System.Threading.Tasks;

using Shared;

namespace Server
{
    public static class MessageHandler
    {
        //Handler on the 'Server' side of the system
        [Route( "/Message[@type='Request' and @action='HeartBeat']" )]
        public static Task<HeartBeatResponseMessage> HandleMessage( HeartBeatRequestMessage request )
        {
            Console.WriteLine( $"Server Received HeartBeatRequestMessage => {request.Id}" );
            var response = new HeartBeatResponseMessage {
                Id = request.Id,
                POSData = request.POSData,
                Result = new Result { Status = Status.Success}
            };

            return Task.FromResult( response );
        }

        //Handler on the 'Server' side of the system
        [Route( "/Message[@type='Request' and @action='SubmitBasket']" )]
        public static Task<SubmitBasketResponse> HandleMessage( SubmitBasketRequest request )
        {
            Console.WriteLine( $"Server Received SubmitBasketRequest => {request.Id}" );
            var response = new SubmitBasketResponse {
                Id = request.Id,
                POSData = request.POSData,
                Result = new Result { Status = Status.Success}
            };

            return Task.FromResult( response );
        }
    }
}
