#nullable   enable

using System.Xml.Serialization;

using Newtonsoft.Json;

namespace Shared
{
    [XmlRoot( "Message" )]
    public class HeartBeatResponseMessage : Message
    {
        [XmlElement( "Result" )]
        [JsonProperty( "result" )]
        public Result? Result { get; set; }

        public HeartBeatResponseMessage( )
        {
            Type = MessageType.Response;
            Action = "HeartBeat";
        }
    }
}
