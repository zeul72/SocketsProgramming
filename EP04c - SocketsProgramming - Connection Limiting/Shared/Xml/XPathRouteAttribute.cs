using System;

namespace Shared.Xml
{
    [AttributeUsage( AttributeTargets.Method, AllowMultiple = false )]
    public class XPathRouteAttribute : RouteAttribute
    {
        public XPathRouteAttribute(string path) : base( path ) { }
    }
}
