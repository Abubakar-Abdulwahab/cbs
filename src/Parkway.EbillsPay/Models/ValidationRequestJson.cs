using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.EbillsPay.Models
{
    public class ValidationRequestJson
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
        public decimal Amount { get; set; }

        /// <summary>
        /// Params object
        /// </summary>
        public Params Params { get; set; }
    }
}
