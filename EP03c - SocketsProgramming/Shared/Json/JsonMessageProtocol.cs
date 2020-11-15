using System.Text;

using Newtonsoft.Json.Linq;

namespace Shared.Json
{

    public class JsonMessageProtocol : Protocol<JObject>
    {
        protected override JObject Decode( byte[ ] message )
            => JsonSerialization.Deserialize( Encoding.UTF8.GetString( message ) );

        protected override byte[ ] EncodeBody<T>( T message ) 
            => Encoding.UTF8.GetBytes( JsonSerialization.Serialize( message ).ToString( ) );

    }
}
