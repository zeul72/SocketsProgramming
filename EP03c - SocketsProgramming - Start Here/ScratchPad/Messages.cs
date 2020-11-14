#nullable   enable

using System.Xml.Serialization;

namespace ScratchPad
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
        public string? Id { get; set; }

        [XmlAttribute( "type" )]
        public MessageType Type { get; set; }

        [XmlAttribute( "action" )]
        public string? Action { get; set; }
    }

    public class POSData
    {
        [XmlAttribute( "id" )]
        public string? Id { get; set; }
    }

    public class Result
    {
        [XmlAttribute( "status" )]
        public Status Status { get; set; }
    }

    

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


    [XmlRoot( "Message" )]
    public class HeartBeatResponseMessage : Message
    {
        [XmlElement( "Result" )]
        public Result? Result { get; set; }

        [XmlElement( "POS" )]
        public POSData? POSData { get; set; }

        public HeartBeatResponseMessage( )
        {
            Type = MessageType.Response;
            Action = "HeartBeat";
        }
    }
}
