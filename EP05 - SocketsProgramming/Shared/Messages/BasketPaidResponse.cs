#nullable enable

using System.Xml.Serialization;

using Newtonsoft.Json;

namespace Shared
{


    [XmlRoot( "Message" )]
    public class BasketPaidResponse : Message
    {

        [XmlAttribute( "posTxnNumber" )]
        public string? POSTransactionNumber { get; set; }

        [XmlElement( "Result" )]
        [JsonProperty( "Result" )]
        public Result? Result { get; set; }

        public BasketPaidResponse( )
        {
            Type = MessageType.Response;
            Action = "BasketPaid";
        }
    }
}
