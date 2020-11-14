#nullable   enable

using System.Xml.Serialization;

using Newtonsoft.Json;

namespace Shared
{
    [XmlRoot( "Message" )]
    public class HeartBeatRequestMessage : Message
    {

        [XmlElement( "POSData" )]
        [JsonProperty( "posData" )]
        public POSData? POSData { get; set; }

        public HeartBeatRequestMessage( )
        {
            Type = MessageType.Request;
            Action = "HeartBeat";
        }
    }
}
