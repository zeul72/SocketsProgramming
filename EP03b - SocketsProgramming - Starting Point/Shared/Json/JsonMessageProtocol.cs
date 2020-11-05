using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Shared {

    public class JsonMessageProtocol : Protocol<JObject> {

        static readonly JsonSerializer          _serializer;
        static readonly JsonSerializerSettings  _settings;
        static JsonMessageProtocol( ) {
            _settings = new JsonSerializerSettings {
                Formatting = Formatting.Indented,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                ContractResolver = new DefaultContractResolver {
                    NamingStrategy = new CamelCaseNamingStrategy {
                        ProcessDictionaryKeys = false
                    }
                }
            };
            _settings.PreserveReferencesHandling = PreserveReferencesHandling.None;
            _serializer = JsonSerializer.Create( _settings );
        }

        protected override JObject Decode( byte[ ] message )
            => JObject.Parse( Encoding.UTF8.GetString( message ) );

        protected override byte[ ] EncodeBody<T>( T message ) {
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            _serializer.Serialize( sw, message );
            return Encoding.UTF8.GetBytes( sb.ToString( ) );
        }
    }
}
