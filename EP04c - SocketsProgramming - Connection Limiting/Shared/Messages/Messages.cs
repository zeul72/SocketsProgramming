#nullable   enable

using System.Xml.Serialization;

using Newtonsoft.Json;

namespace Shared
{
    /*
      *  <Message type='Request' action='HeartBeat' id='0001'>
      *    <POS id='POS_001'/>
      *  </Message>
      * 
      * <Message type='Response' action='HeartBeat' id='0001'>
      *   <POS id='POS_001'/> 
      *   <Result status='Success'/>
      * </Message>
      * 
      */

    public enum MessageType
    {
        Request,
        Response
    }

    public enum Status
    {
        Success,
        Failure
    }

    [XmlRoot( "Message" )]
    public abstract class Message
    {
        [XmlAttribute( "id" )]
        [JsonProperty( "id" )]
        public string? Id { get; set; }

        [XmlAttribute( "type" )]
        [JsonProperty( "type" )]
        public MessageType Type { get; set; }

        [XmlAttribute( "action" )]
        [JsonProperty( "action" )]
        public string? Action { get; set; }

        [XmlElement( "POSData" )]
        [JsonProperty( "posData" )]
        public POSData? POSData { get; set; }
    }

    public class POSData
    {
        [XmlAttribute( "id" )]
        [JsonProperty( "id" )]
        public string? Id { get; set; }
    }

    public class Result
    {
        [XmlAttribute( "status" )]
        [JsonProperty( "status" )]
        public Status Status { get; set; }
    }
}
