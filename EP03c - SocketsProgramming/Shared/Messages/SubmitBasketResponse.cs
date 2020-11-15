#nullable   enable

using System.Xml.Serialization;

using Newtonsoft.Json;

namespace Shared
{
    [XmlRoot( "Message" )]
    public class SubmitBasketResponse : Message
    {
        [XmlElement( "Result" )]
        [JsonProperty( "Result" )]
        public Result? Result { get; set; }

        [XmlElement( "POSData" )]
        [JsonProperty( "posData" )]
        public POSData? POSData { get; set; }

        public SubmitBasketResponse( )
        {
            Type = MessageType.Response;
            Action = "SubmitBasket";
        }
    }
}
