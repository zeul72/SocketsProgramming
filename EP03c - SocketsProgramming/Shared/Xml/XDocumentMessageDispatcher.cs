#nullable enable

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Shared.Xml
{
    public class XDocumentMessageDispatcher : MessageDispatcher<XDocument>
    {
        public override void Register<TParam, TResult>( Func<TParam, Task<TResult>> target )
        {
            if ( !HasRouteAttribute( target.Method ) )
                throw new Exception( "Missing Route Attribute" );

            var wrapper = new Func<XDocument,Task<XDocument?>>( async xml => {
                var @param = XmlSerialization.Deserialize<TParam>(xml);
                var result = await target(param);

                if(result != null)
                    return XmlSerialization.Serialize(result);
                else
                    return null;
            });

#pragma warning disable CS8604 // Possible null reference argument.
            AddHandler( GetRouteAttribute( target.Method ), wrapper );
#pragma warning restore CS8604 // Possible null reference argument.

        }

        public override void Register<TParam>( Func<TParam, Task> target )
        {
            if ( !HasRouteAttribute( target.Method ) )
                throw new Exception( "Missing Route Attribute" );

            var wrapper = new Func<XDocument,Task<XDocument?>>( async xml => {
                var @param = XmlSerialization.Deserialize<TParam>(xml);
                await target(@param);
                return null;
            });


#pragma warning disable CS8604 // Possible null reference argument.
            AddHandler( GetRouteAttribute( target.Method ), wrapper );
#pragma warning restore CS8604 // Possible null reference argument.
        }

        protected override XDocument Serialize( Type type, object obj )
            => XmlSerialization.Serialize( type, obj );
        protected override object Deserialize( Type target, XDocument data )
            => XmlSerialization.Deserialize( target, data );

        protected override RouteAttribute? GetRouteAttribute( MemberInfo mi )
            => mi.GetCustomAttribute<XPathRouteAttribute>( );
        protected override bool IsMatch( RouteAttribute route, XDocument message )
         => ( message.XPathEvaluate( $"boolean({route.Path})" ) as bool? ) ?? false;

    }
}
