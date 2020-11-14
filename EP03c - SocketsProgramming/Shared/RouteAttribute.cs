using System;

namespace Shared
{
    [AttributeUsage( AttributeTargets.Method )]
    public abstract class RouteAttribute : Attribute
    {
        public string Path { get; }

        public RouteAttribute( string path ) => Path = path;
    }
}
