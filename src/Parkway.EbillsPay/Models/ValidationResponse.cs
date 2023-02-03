using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Parkway.EbillsPay.Models
{
    public class ValidationResponse
    {
        /// <summary>
        /// Unique identifier of the biller
        /// </summary>
        public string BillerID { get; set; }

        /// <summary>
        /// The number of the next form that will be displayed when the current form is submitted.
        /// </summary>
        public string NextStep { get; set; }

        /// <summary>
        /// Indicates the status of the transaction.
        /// </summary>
        public string ResponseCode { get; set; }

        /// <summary>
        /// Payment detail
        /// </summary>
        public PaymentDetail PaymentDetail { get; set; }

        /// <summary>
        /// List of form fields
        /// </summary>
        [XmlElement(ElementName = "Param")]
        public List<Param> Param { get; set; }
    }
}
