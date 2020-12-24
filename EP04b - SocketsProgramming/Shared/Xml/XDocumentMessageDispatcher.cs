
#nullable enable

using System;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Shared.Xml
{
    public class XDocumentMessageDispatcher : MessageDispatcher<XDocument>
    {
        protected override RouteAttribute? GetRouteAttribute( MethodInfo mi ) => mi.GetCustomAttribute<XPathRouteAttribute>( );
        protected override bool IsMatch( RouteAttribute route, XDocument message )
            => ( message.XPathEvaluate( $"boolean({route.Path})" ) as bool? ) ?? false;


        protected override TParam Deserialize<TParam>( XDocument message )
            => XmlSerialization.Deserialize<TParam>( message );

        protected override XDocument? Serialize<T>( T instance )
            => XmlSerialization.Serialize( instance );

        protected override object Deserialize( Type paramType, XDocument message )
            => XmlSerialization.Deserialize( paramType, message );
    }
}
