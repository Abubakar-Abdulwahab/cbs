using System.Xml.Serialization;
using System.Collections.Generic;

namespace Parkway.ThirdParty.Payment.Processor.Models
{
    [XmlRoot(ElementName = "PaymentNotificationResponse")]
    public class PaymentNotificationResponse
    {
        [XmlArray("Payments")]
        [XmlArrayItem("Payment")]
        public List<PaymentResponse> Payments { get; set; }
    }

    [XmlRoot(ElementName = "Payment")]
    public class PaymentResponse
    {
        [XmlElement("Status")]
        /// <summary>
        /// Acknowledgement returned by Merchant to indicate if payment was received or not 0=Received/Duplicate Payment 1= Rejected by System
        /// </summary>
        public int Status { get; set; }

        [XmlElement("StatusMessage")]
        /// <summary>
        /// Description of the Status
        /// </summary>
        public string StatusMessage { get; set; }

        [XmlElement("PaymentLogId")]
        /// <summary>
        /// Payment Log ID
        /// </summary>
        public string PaymentLogId { get; set; }
    }
}
