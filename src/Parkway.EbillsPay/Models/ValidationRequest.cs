using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Parkway.EbillsPay.Models
{
    public class ValidationRequest
    {
        /// <summary>
        /// Unique code that identifies the Bank where the payment was made
        /// </summary>
        public string SourceBankCode { get; set; }

        /// <summary>
        /// Bank Name
        /// </summary>
        public string SourceBankName { get; set; }

        /// <summary>
        /// Indicates if the field is mandatory
        /// </summary>
        public string InstitutionCode { get; set; }

        /// <summary>
        /// Identifies the channel where the transaction was initiated. This can be internet banking, ATM etc.
        /// </summary>
        public string ChannelCode { get; set; }

        /// <summary>
        /// A number identifying the step assigned to the form. E.g. if the value is 1, it means it is the first form the customer sees,
        /// 2 means the second form etc.
        /// </summary>
        public string Step { get; set; }

        /// <summary>
        /// The number of steps (forms) available for the current product.
        /// </summary>
        public string StepCount { get; set; }

        /// <summary>
        /// Name of the Customer making payment
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// Account number of the Customer making payment
        /// </summary>
        public string CustomerAccountNumber { get; set; }

        /// <summary>
        /// Unique identifier of the biller
        /// </summary>
        public string BillerID { get; set; }

        /// <summary>
        /// Name of the biller
        /// </summary>
        public string BillerName { get; set; }

        /// <summary>
        /// Unique identifier of the product
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// Name of the product
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// This can be the amount the customer paid (for validation request) or 
        /// the amount that goes to Biller’s account after transaction fee is subtracted 
        /// from the amount paid by customer (for notification request).
        /// </summary>
        public string Amount { get; set; }

        /// <summary>
        /// List of form fields
        /// </summary>
        [XmlElement(ElementName = "Param")]
        public List<Param> Param { get; set; }
    }
}
