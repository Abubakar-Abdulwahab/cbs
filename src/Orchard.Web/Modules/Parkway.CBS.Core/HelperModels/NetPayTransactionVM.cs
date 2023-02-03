using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class NetPayTransactionVM
    {
        /// <summary>
        /// Code is to know if the transaction was successful
        /// 00 means succssful
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Amount the user paid in kobo
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Description of the status of the transaction
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Unique reference number CBS sent to NetPay
        /// </summary>
        public string MerchantRef { get; set; }

        /// <summary>
        /// NetPay returned payment reference number
        /// </summary>
        public string TransactionRef { get; set; }

        /// <summary>
        /// This shows the last action performed during the payment processing
        /// E.g Enter Pin or Approved which means all actions carried out
        /// </summary>
        public string NextAction { get; set; }

        /// <summary>
        /// NetPay returned payment reference number
        /// </summary>
        public string PaymentRef { get; set; }

        /// <summary>
        /// For request validation during background payment notification
        /// </summary>
        public string HMac { get; set; }

        /// <summary>
        /// Currency code of the payment
        /// </summary>
        public string CurrencyCode { get; set; }

        /// <summary>
        /// Date the payment was successful
        /// Format "yyyy-MM-ddTHH:mm:ss.fffZ"
        /// </summary>
        public string PaymentDate { get; set; }

        /// <summary>
        /// Date the payment was initiated
        /// Format "yyyy-MM-ddTHH:mm:ss.fffZ"
        /// </summary>
        public string TransactionDate { get; set; }

    }
}