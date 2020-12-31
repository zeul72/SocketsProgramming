
using Newtonsoft.Json.Linq;

namespace Shared.Json
{
    public class JsonSocketServer
        : SocketServer<JsonChannel, JsonMessageProtocol, JObject, JsonMessageDispatcher>
    {
        public JsonSocketServer( int maxConnections = 100000000 )
            : base( maxConnections ) { }
    }
}
