#nullable   enable

using System.Xml.Serialization;

using Newtonsoft.Json;

namespace Shared
{
    [XmlRoot( "Message" )]
    public class SubmitBasketRequest : Message
    {

        public SubmitBasketRequest( )
        {
            Type = MessageType.Request;
            Action = "SubmitBasket";
        }
    }
}
