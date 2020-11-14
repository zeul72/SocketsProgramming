#nullable   enable

using System.Xml.Serialization;

using Newtonsoft.Json;

namespace Shared
{
    [XmlRoot( "Message" )]
    public class SubmitBasketRequest : Message
    {

        [XmlElement( "POSData" )]
        [JsonProperty( "posData" )]
        public POSData? POSData { get; set; }

        public SubmitBasketRequest( )
        {
            Type = MessageType.Request;
            Action = "SubmitBasket";
        }
    }
}
