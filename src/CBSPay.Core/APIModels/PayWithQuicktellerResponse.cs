using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBSPay.Core.APIModels
{
    public class PayWithQuicktellerResponse
    {
        public string resp_code { get; set; }
        public string resp_desc { get; set; }
        public string tx_ref { get; set; }
        public string signature { get; set; }
    }

    public class QuicktellerAPIResponse
    {
        //Response Code returned by the Service
        public string ResponseCode { get; set; }
        //
        public string ResponseDescription { get; set; }
        //amount paid by the customer in kobo
        public decimal Amount { get; set; } //Returned if response is successful
        //Date and time of the transaction
        public DateTime TransactionDate { get; set; } //passed in the header
        //Quickteller generated unique ID for the payment (also referred to as the TransactionReference)
        public string PaymentReference { get; set; }
    }
}
