using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Parkway.ThirdParty.Payment.Processor.Models
{
    [XmlRoot(ElementName = "CustomerInformationResponse")]
    public class CustomerInformationResponse : BaseResponse
    {
        [XmlElement(IsNullable = false)]
        public string MerchantReference { get; set; }

        [XmlArray("Customers")]
        [XmlArrayItem("Customer")]
        public List<Customer> Customers { get; set; }
    }


    [XmlRoot(ElementName = "Customer")]
    public class Customer
    {
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

        [XmlElement(IsNullable = false)]
        public string Phone { get; set; }

        [XmlElement(IsNullable = false)]
        public string Email { get; set; }

        [XmlElement(IsNullable = false)]
        public string ThirdPartyCode { get; set; }

        [XmlElement("Amount")]
        public decimal Amount { get; set; }

        [XmlArray("PaymentItems")]
        [XmlArrayItem("Item")]
        public List<Item> PaymentItems { get; set; }
    }

    [XmlRoot(ElementName = "Item")]
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
}
