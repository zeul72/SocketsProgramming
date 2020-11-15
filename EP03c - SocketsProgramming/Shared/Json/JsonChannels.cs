
using Newtonsoft.Json.Linq;

namespace Shared.Json
{
    public class JsonChannel : Channel<JsonMessageProtocol, JObject> { }

    public class JsonClientChannel : ClientChannel<JsonMessageProtocol, JObject> { }

}
