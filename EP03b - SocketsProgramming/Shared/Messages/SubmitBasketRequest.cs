#nullable   enable

using System.Xml.Serialization;

namespace Shared
{
    [XmlRoot( "Message" )]
    public class SubmitBasketRequest : Message
    {

        [XmlElement( "POS" )]
        public POSData? POSData { get; set; }

        public SubmitBasketRequest( )
        {
            Type = MessageType.Request;
            Action = "SubmitBasket";
        }
    }
}
