using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.ThirdParty.Payment.Processor.Models
{
    public class CustomerInformationRequest : BaseRequest
    {
        /// <summary>
        /// Mandatory - Third party platform service url
        /// </summary>
        public string ServiceUrl { get; set; }

        /// <summary>
        /// Username to access Service at Merchant’s end
        /// </summary>
        public string ServiceUsername { get; set; }

        /// <summary>
        /// Password to access Service at Merchant’s end
        /// </summary>
        public string ServicePassword { get; set; }

        /// <summary>
        /// Mandatory - Unique ID given by Interswitch to the client using the bill payment solution
        /// </summary>
        public string MerchantReference { get; set; }

        /// <summary>
        /// Mandatory - Unique ID for the paying customer or specific order. Typically provided to the paying customer by the merchant
        /// </summary>
        public string CustReference { get; set; }

        /// <summary>
        /// This is an agreed code indicating the item that the customer has indicated he wants to pay for.
        /// </summary>
        public string PaymentItemCode { get; set; }

        public string ThirdPartyCode { get; set; }

        /// <summary>
        /// Agreed code used to group items customers can pay for
        /// </summary>
        public string PaymentItemCategoryCode { get; set; }

        /// <summary>
        /// Amount to be paid, 0 if customer is allowed to pay any amount
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Unique identifier for Terminal
        /// </summary>
        public string TerminalId { get; set; }
    }
}
