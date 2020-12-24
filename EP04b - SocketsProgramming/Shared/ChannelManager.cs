using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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

        const int GROOMING_INTERVAL_MINUTES = 1;    //should come from config/commandline/etc.
        private readonly System.Timers.Timer _groomer = new System.Timers.Timer(GROOMING_INTERVAL_MINUTES * 60 *1000);

        public ChannelManager( Func<IChannel> channelFactory )
        {
            _channelFactory = channelFactory;

            _groomer.Elapsed += OnGroomerElapsedEventHandler;
            _groomer.Start( );

        }

        private void OnGroomerElapsedEventHandler( object sender, System.Timers.ElapsedEventArgs e )
        {

            _groomer.Stop( );
            Console.WriteLine( "BEGIN SOCKET GROOMING" );
            int socketsGroomed = 0;
            try {
                var deadChannels = new List<Guid>();


                var delta = DateTime.UtcNow.Subtract( new TimeSpan(0,GROOMING_INTERVAL_MINUTES,0) );

                foreach ( var k in _channels.Keys ) {
                    //get the channel and look at last sent/received to see if channel is alive
                    var c = _channels[k];
                    var mostRecent = DateTime.Compare(c.LastReceived, c.LastSent) > 0 ? c.LastReceived : c.LastSent;
                    if ( DateTime.Compare( delta, mostRecent ) < 0 )
                        deadChannels.Add( k );
                }

                //remove all dead channels
                foreach ( var k in deadChannels ) {
                    Console.WriteLine( $"Closing/Removing Dead Channel {k}" );
                    var c = _channels[ k ];
                    c.Close( );
                    socketsGroomed++;
                }

            } finally {
                _groomer.Start( );
            }

            Console.WriteLine( $"END SOCKET GROOMING : {socketsGroomed} Sockets Groomed" );

        }

        public void Accept(Socket socket)
        {
            var channel = _channelFactory();
            _channels.TryAdd( channel.Id, channel );
            channel.Closed += ( s, e ) => _channels.TryRemove( channel.Id, out var _ );
            channel.Attach( socket );   //
        }

    }
}
