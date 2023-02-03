using CBSPay.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBSPay.Core.APIModels
{
    /// <summary>
    /// API model for EIR-InterswitchS Web Payment Request
    /// </summary>
    public class PayWithQuicktellerRequest
    {
        /// <summary>
        /// the merchant’s payment code on the Quickteller platform
        /// </summary>
        public string PaymentCode { get; set; }
        /// <summary>
        /// value I expect to be paid. In lower denomination. (N10 = 1000)
        /// </summary>
        public decimal Amount { get; set; }
        public ButtonSize ButtonSize { get; set; }
        public string CustomerId { get; set; }
        //phone number for the paying customer.
        public string MobileNumber { get; set; }
        //email address for the paying cutomer.
        public string EmailAddress { get; set; }
        //the page on my web application that I want the payment gateway to redirect to after the payment is complete.
        public string RedirectUrl { get; set; }
        public string RequestReference { get; set; }
    }
}
