
#nullable enable

using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

using Shared;

namespace Server
{


    //1. Map POS Identifer to IChannel :: via the Heartbeat
    //2. Provide SendTo<T> abstraction to send message to POS by POS Id
    public class POSController
    {

        readonly ConcurrentDictionary<string,WeakReference<IChannel>> _posChannelMap = new ConcurrentDictionary<string, WeakReference<IChannel>>();

        public void ProcessHeartBeat( string posId, IChannel channel )
        {
            var wr = new WeakReference<IChannel>(channel);
            _posChannelMap.AddOrUpdate( posId, wr, ( k, v ) => wr );
        }

        public async Task SendTo<T>(T message) where T : Message
        {
            var posId = message.POSData?.Id;
            if ( string.IsNullOrWhiteSpace( posId ) )
                throw new Exception( "POS ID Must be included in Message" );


            if(_posChannelMap.TryGetValue(posId, out var wr)) {
                if(wr.TryGetTarget(out var channel)) {
                    await channel.SendAsync( message ).ConfigureAwait( false );
                } else {
                    //channel is dead, what to do?
                    _posChannelMap.TryRemove( posId, out var _ );
                }
            }

        }

    }
}
