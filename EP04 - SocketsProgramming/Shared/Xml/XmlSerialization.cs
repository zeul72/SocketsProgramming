using System;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Shared.Xml
{
    public static class XmlSerialization
    {
        public static XDocument Serialize<T>( T instance ) => Serialize( typeof( T ), instance );
        public static XDocument Serialize( Type targetType, object instance )
        {
            using var ms = new MemoryStream();
            var xs = new XmlSerializer(targetType);
            xs.Serialize( ms, instance );
            ms.Flush( );
            ms.Position = 0L;
            return XDocument.Load( ms );
        }

        public static T Deserialize<T>( XDocument xml ) => ( T )Deserialize( typeof( T ), xml );
        public static object Deserialize( Type targetType, XDocument xml ) => new XmlSerializer( targetType ).Deserialize( new StringReader( xml.ToString( ) ) );

    }
}
