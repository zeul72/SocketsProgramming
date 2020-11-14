
using Newtonsoft.Json.Linq;

namespace Shared
{
    public class JsonChannel : Channel<JsonMessageProtocol, JObject> { }
    public class JsonClientChannel : ClientChannel<JsonMessageProtocol, JObject> { }

}
