using System;
using System.Collections.Concurrent;
using System.Net.Sockets;

namespace Shared
{

    //What does this do?
    //1. Track a list of all active client connections
    //2. 'Grooming' of channels - aka House Keeping
    //3. Connection Limiting 
    public class ChannelManager
    {
        readonly ConcurrentDictionary<Guid,IChannel> _channels = new ConcurrentDictionary<Guid, IChannel>();
        readonly Func<IChannel>                     _channelFactory;

        public ChannelManager( Func<IChannel> channelFactory )
            => _channelFactory = channelFactory;


        public void Accept(Socket socket)
        {
            var channel = _channelFactory();
            _channels.TryAdd( channel.Id, channel );
            channel.Attach( socket );   //
        }

    }
}
