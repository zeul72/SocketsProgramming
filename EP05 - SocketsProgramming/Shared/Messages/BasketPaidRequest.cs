#nullable enable

using System.Xml.Serialization;

namespace Shared
{

    [XmlRoot( "Message" )]
    public class BasketPaidRequest : Message
    {

        [XmlAttribute( "posTxnNumber" )]
        public string? POSTransactionNumber { get; set; }

        [XmlElement( "PaymentInformation" )]
        public PaymentInfo? PaymentInfo { get; set; }


        public BasketPaidRequest( )
        {
            Type = MessageType.Request;
            Action = "BasketPaid";
        }
    }
}
