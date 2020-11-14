using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Shared {

    public class ClientChannel<TProtocol, TMessageType> : Channel<TProtocol, TMessageType>
        where TProtocol : Protocol<TMessageType>, new() {

        public async Task ConnectAsync( IPEndPoint endPoint ) {
            var socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            await socket.ConnectAsync( endPoint ).ConfigureAwait( false );

            Attach( socket );
        }
    }
}
