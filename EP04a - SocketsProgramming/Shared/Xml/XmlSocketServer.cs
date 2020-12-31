using System.Xml.Linq;

namespace Shared.Xml
{
    public class XmlSocketServer
        : SocketServer<XmlChannel, XmlMessageProtocol, XDocument, XDocumentMessageDispatcher>
    { }
}
