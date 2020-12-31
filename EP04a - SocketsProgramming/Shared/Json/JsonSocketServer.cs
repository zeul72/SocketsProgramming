
using Newtonsoft.Json.Linq;

namespace Shared.Json
{
    public class JsonSocketServer
        : SocketServer<JsonChannel, JsonMessageProtocol, JObject, JsonMessageDispatcher>
    { }
}
