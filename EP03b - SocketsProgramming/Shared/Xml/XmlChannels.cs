using System.Xml.Linq;

namespace Shared.Xml
{

    public class XmlChannel : Channel<XmlMessageProtocol, XDocument> { }

    public class XmlClientChannel : ClientChannel<XmlMessageProtocol, XDocument> { }

}
