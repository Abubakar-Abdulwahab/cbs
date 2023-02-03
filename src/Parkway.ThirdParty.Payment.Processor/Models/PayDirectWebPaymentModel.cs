using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.ThirdParty.Payment.Processor.Models
{
    /// <summary>
    /// Form Creation Model for interswitch
    /// https://sandbox.interswitchng.com/docbase/docs/webpay-direct-paydirect-web/transaction-payment-leg/form-creation/
    /// </summary>
    public class PayDirectWebPaymentFormModel
    {
        public string ProductId { get; set; }

        public string PayItemId { get; set; }

        public string Amount { get; set; }

        public string Currency { get; set; }

        public string SiteRedirectURL { get; set; }

        public string TxnRef { get; set; }

        public string CustId { get; set; }

        public string Hash { get; set; }

        public string ActionURL { get; set; }

        public string PayDirectURL { get; set; }

        public string CustomerName { get; set; }
    }    
}
