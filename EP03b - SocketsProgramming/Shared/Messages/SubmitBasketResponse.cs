#nullable   enable

using System.Xml.Serialization;

namespace Shared
{
    [XmlRoot( "Message" )]
    public class SubmitBasketResponse : Message
    {
        [XmlElement( "Result" )]
        public Result? Result { get; set; }

        [XmlElement( "POS" )]
        public POSData? POSData { get; set; }

        public SubmitBasketResponse( )
        {
            Type = MessageType.Response;
            Action = "SubmitBasket";
        }
    }
}
