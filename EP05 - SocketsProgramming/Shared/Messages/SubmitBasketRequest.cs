#nullable   enable

using System.Xml.Serialization;

using Newtonsoft.Json;

namespace Shared
{
    [XmlRoot( "Message" )]
    public class SubmitBasketRequest : Message
    {
        [XmlAttribute( "posTxnNumber" )]
        public string? POSTransactionNumber { get; set; }

        public SubmitBasketRequest( )
        {
            Type = MessageType.Request;
            Action = "SubmitBasket";
        }
    }
}
