using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.ThirdParty.Payment.Processor.Models
{
    /// <summary>
    /// https://sandbox.interswitchng.com/docbase/docs/webpay-direct-paydirect-web/transaction-confirmation-leg/server-response/
    /// </summary>
    public class PayDirectWebServerResponse
    {
        /// <summary>
        /// Alphanumeric Code indicating the status of the transaction.
        /// Refer to the Response Codes for a list of possible responses.
        /// </summary>
        public string ResponseCode { get; set; }

        /// <summary>
        /// Brief description of result 
        /// </summary>
        public string ResponseDescription { get; set; }

        /// <summary>
        /// Approved amount in kobo
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// The transaction reference 
        /// </summary>
        public string MerchantReference { get; set; }

        /// <summary>
        /// Payment ref
        /// </summary>
        public string PaymentReference { get; set; }

        /// <summary>
        /// Date and time transaction took place 
        /// </summary>
        public DateTime TransactionDate { get; set; }


        public string RetrievalReferenceNumber { get; set; }

        public string CardNumber { get; set; }
    }
}
