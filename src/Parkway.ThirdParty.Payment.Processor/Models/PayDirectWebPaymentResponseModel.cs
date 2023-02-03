using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.ThirdParty.Payment.Processor.Models
{
    /// <summary>
    /// https://sandbox.interswitchng.com/docbase/docs/webpay-direct-paydirect-web/transaction-payment-leg/sample-response-fields/
    /// </summary>
    public class PayDirectWebPaymentResponseModel
    {
        /// <summary>
        /// Will always have a value of 0 
        /// </summary>
        public string apprAmt { get; set; }

        /// <summary>
        /// Will always have a value of 0 
        /// </summary>
        public string cardNum { get; set; }

        /// <summary>
        /// A reference number that uniquely identifies all transactions that goes through the gateway
        /// </summary>
        public string payRef { get; set; }

        /// <summary>
        /// Reference number from the switch’s interconnecting with the banking application
        /// </summary>
        public string retRef { get; set; }

        /// <summary>
        /// The transaction Reference initially generated & sent by the merchant site will be sent back with this variable 
        /// </summary>
        public string txnref { get; set; }
    }
}
