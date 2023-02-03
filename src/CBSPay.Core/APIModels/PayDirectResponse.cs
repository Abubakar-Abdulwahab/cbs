using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CBSPay.Core.ViewModels
{
    [XmlRoot(Namespace = "")]
    public class PaymentNotificationResponse
    {
        //[XmlElement("Payments")]
        [XmlArray("Payments")]
        [XmlArrayItem("Payment")]
        public List<PaymentResponse> Payments { get; set; }
    }

    [XmlRoot(ElementName = "CustomerInformationResponse")]
    public class CustomerInformationResponse
    {
        [XmlElement(IsNullable = false)]
        public string MerchantReference { get; set; }
        [XmlElement(ElementName ="Customers")]
        public List<Customer> Customers { get; set; }
    }

    
    public class Customer
    {
        /// <summary>
        /// Acknowledgement returned by Merchant to indicate if payment was received or not 0=Received/Duplicate Payment 1= Rejected by System
        /// </summary>
        [XmlElement("Status")]
        public int Status { get; set; }
        [XmlElement(IsNullable = false)]
        public string StatusMessage { get; set; }
        [XmlElement("CustReference")]
        public string CustReference { get; set; }
        [XmlElement(IsNullable = false)]
        public string CustomerAlternateReference { get; set; }
        [XmlElement("FirstName")]
        public string FirstName { get; set; }
        //[XmlElement("Phone")]
        [XmlElement(IsNullable = false)]
        public string Phone { get; set; }
        //[XmlElement("Email")]
        [XmlElement(IsNullable = false)]
        public string Email { get; set; }
        //[XmlElement("ThirdPartyCode")]
        [XmlElement(IsNullable = false)]
        public string ThirdPartyCode { get; set; }
        [XmlElement("Amount")]
        public decimal Amount { get; set; }
        [XmlElement("PaymentItems")]
        public List<Item> PaymentItems { get; set; }
    }

    public class Item
    {
        [XmlElement(IsNullable = false)]
        public string ProductName { get; set; }
        [XmlElement(IsNullable = false)]
        public string ProductCode { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
    }

    public class PaymentResponse
    {
        /// <summary>
        /// Acknowledgement returned by Merchant to indicate if payment was received or not 0=Received/Duplicate Payment 1= Rejected by System
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// Payment Log ID
        /// </summary>
        public string PaymentLogId { get; set; }
    }
}
