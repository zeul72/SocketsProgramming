using System.Xml.Linq;

namespace Shared
{

    public class XmlChannel : Channel<XmlMessageProtocol, XDocument> { }

    public class XmlClientChannel : ClientChannel<XmlMessageProtocol, XDocument> { }

}
