#nullable enable

using System;
using System.Reflection;

using Newtonsoft.Json.Linq;

namespace Shared.Json
{
    public class JsonMessageDispatcher : MessageDispatcher<JObject>
    {

        protected override TParam Deserialize<TParam>( JObject message )
             => JsonSerialization.Deserialize<TParam>( message );

        protected override object Deserialize( Type paramType, JObject message )
            => JsonSerialization.ToObject( paramType, message );

        protected override RouteAttribute? GetRouteAttribute( MethodInfo mi )
            => mi.GetCustomAttribute<JsonRouteAttribute>( );

        protected override bool IsMatch( RouteAttribute route, JObject message )
            => message.SelectToken( route.Path )?.ToString( ) == ( route as JsonRouteAttribute )?.Value;

        protected override JObject? Serialize<T>( T instance )
            => JsonSerialization.Serialize( instance );
    }
}
