
using System;
using System.Reflection;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

namespace Shared.Json
{
    public class JsonMessageDispatcher : MessageDispatcher<JObject>
    {
        public override void Register<TParam, TResult>( Func<TParam, Task<TResult>> target )
        {
            throw new NotImplementedException( );
        }

        public override void Register<TParam>( Func<TParam, Task> target )
        {
            throw new NotImplementedException( );
        }

        protected override object Deserialize( Type target, JObject data ) => data.ToObject( target );
        protected override JObject Serialize( Type _, object obj ) => JsonSerialization.Serialize( obj );

        protected override RouteAttribute GetRouteAttribute( MemberInfo mi )
            => mi.GetCustomAttribute<JsonRouteAttribute>( );

        protected override bool IsMatch( RouteAttribute route, JObject message )
            => message.SelectToken( route.Path )?.ToString( ) == ( route as JsonRouteAttribute )?.Value;

    }
}
