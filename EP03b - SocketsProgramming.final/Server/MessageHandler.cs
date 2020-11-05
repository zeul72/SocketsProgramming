#nullable enable

using System;
using System.Threading.Tasks;

using Shared;

namespace Server
{
    public class MessageHandler
    {

        //Handler on the 'Server' side of the system
        [Route( "/Message[@type='Request' and @action='HeartBeat']" )]
        public static Task<HeartBeatResponseMessage> HeartBeatRequestHandler( HeartBeatRequestMessage request )
        {
            Console.WriteLine( $"Server::MessageHandler Received HeartBeatRequestMessage :: Request ID { request.Id }" );
            var response = new HeartBeatResponseMessage {
                Id = request.Id,
                POSData = request.POSData,
                Result = new Result { Status = Status.Success }
            };

            return Task.FromResult( response );
        }
    }
}
