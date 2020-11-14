#nullable   enable

using System.Xml.Serialization;

namespace Shared
{
    [XmlRoot( "Message" )]
    public class HeartBeatRequestMessage : Message
    {

        [XmlElement( "POS" )]
        public POSData? POSData { get; set; }

        public HeartBeatRequestMessage( )
        {
            Type = MessageType.Request;
            Action = "HeartBeat";
        }
    }
}
