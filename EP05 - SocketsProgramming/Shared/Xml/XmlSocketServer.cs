using System.Xml.Linq;

namespace Shared.Xml
{
    public class XmlSocketServer
        : SocketServer<XmlChannel, XmlMessageProtocol, XDocument, XDocumentMessageDispatcher>
    {
        public XmlSocketServer( int maxConnections = 100000000 )
            : base( maxConnections ) { }
    }
}
