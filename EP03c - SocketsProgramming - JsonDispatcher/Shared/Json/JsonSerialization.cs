
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Shared
{
    public static class JsonSerialization
    {
        static readonly JsonSerializer          _serializer;
        static readonly JsonSerializerSettings  _settings;
        static JsonSerialization( )
        {
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
        public static JObject Serialize( object @object ) => JObject.FromObject( @object, _serializer );
        public static JObject Deserialize( string json ) => JObject.Parse( json );
    }
}
