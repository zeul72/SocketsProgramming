using System;
using System.Reflection;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Shared
{
    public class JsonMessageDispatcher : MessageDispatcher<JObject>
    {
        static readonly JsonSerializer _jsonSerializer = new JsonSerializer();

        public override void Register<TParam, TResult>( Func<TParam, Task<TResult>> target )
        {
            throw new NotImplementedException( );
        }

        public override void Register<TParam>( Func<TParam, Task> target )
        {
            throw new NotImplementedException( );
        }

        protected override object Deserialize( Type target, JObject data ) => data.ToObject( target, _jsonSerializer );

        protected override bool IsMatch( RouteAttribute route, JObject message )
        {
            if ( route is JsonRouteAttribute jra ) {
                return message.SelectToken( jra.Path )?.ToString( ) == jra.Value;
            } else {
                return false;
            }
        }

        protected override JObject Serialize( Type type, object obj )
        {
            try {
                return JObject.FromObject( obj, _jsonSerializer );
            } catch(Exception _e) {
                Console.WriteLine( _e );
                throw;
            }
        }

        protected override RouteAttribute GetRouteAttribute( MemberInfo mi ) => mi.GetCustomAttribute<JsonRouteAttribute>( );
    }
}
