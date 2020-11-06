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
        readonly List<(string xpathExpression, Func<XDocument,Task<XDocument?>> targetMethod)> _handlers = new List<(string xpathExpression, Func<XDocument, Task<XDocument?>> targetMethod)>();

        public override async Task<XDocument?> DispatchAsync( XDocument message )
        {
            foreach ( var (xpath, target) in _handlers )
            {
                if ( ( message.XPathEvaluate( xpath ) as bool? ) == true )
                {
                    return await target( message );
                }
            }
            //No handler?? what to do??
            return null;
        }

        public override void Register<TParam, TResult>( Func<TParam, Task<TResult>> target )
        {
            var xpathRouteExpression = GetXPathRoute(target.Method);

            var wrapper = new Func<XDocument,Task<XDocument?>>( async xml => {
                var @param = XmlSerialization.Deserialize<TParam>(xml);
                var result = await target(param);

                if(result != null)
                    return XmlSerialization.Serialize(result);
                else
                    return null;
            });

            _handlers.Add( (xpathRouteExpression, wrapper) );

        }

        public override void Register<TParam>( Func<TParam, Task> target )
        {
            var xpathRouteExpression = GetXPathRoute(target.Method);

            var wrapper = new Func<XDocument,Task<XDocument?>>( async xml => {
                var @param = XmlSerialization.Deserialize<TParam>(xml);
                await target(@param);
                return null;
            });

            _handlers.Add( (xpathRouteExpression, wrapper) );
        }

        string GetXPathRoute( MethodInfo methodInfo )
        {
            var routeAttribute = methodInfo.GetCustomAttribute<RouteAttribute>();
            if ( routeAttribute == null )
                throw new ArgumentException( $"Method {methodInfo.Name} missing required RouteAttribute" );
            return $"boolean({routeAttribute.Path})";
        }
    }
}
