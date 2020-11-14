using System;

namespace Shared
{
    [AttributeUsage( AttributeTargets.Method, AllowMultiple = false )]
    public class JsonRouteAttribute : RouteAttribute
    {
        public string Value { get; }
        public JsonRouteAttribute( string path, string value )
            : base( path ) => Value = value;
    }
}
