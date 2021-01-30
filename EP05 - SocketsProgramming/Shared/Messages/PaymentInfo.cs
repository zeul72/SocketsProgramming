#nullable enable

using System.Xml.Serialization;

namespace Shared
{
    public class PaymentInfo
    {
        [XmlAttribute( "authCode" )]
        public string? AuthorizationCode { get; set; }

        [XmlAttribute( "amount" )]
        public decimal Amount { get; set; }

        [XmlAttribute( "lastFour" )]
        public string? LastFour { get; set; }

    }
}
