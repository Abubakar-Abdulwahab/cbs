using CBSPay.Core.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBSPay.Core.APIModels
{
    /// <summary>
    /// contains details of the taxpayer payment notofication
    /// </summary>
    public class PaymentDetails
    { 
        public string ReferenceNumber { get; set; }
        public string RIN { get; set; }
        public string TIN { get; set; }
        public string PhoneNumber { get; set; }
        public PaymentChannel PaymentChannel { get; set; }
        [Required]
        [Range (0,int.MaxValue, ErrorMessage ="Amount cannot be less 0")]
        public decimal AmountPaid { get; set; }
        public decimal ReferenceAmount { get; set; }
        public string PaymentIdentifier { get; set; }
        public string PaymentReference { get; set; }
        public string ReceiptNo { get; set; }
        public DateTime DatePaid { get; set; }
        public string TaxPayerName { get; set; }
        public string SettlementNotes { get; set; }
        public bool IsCustomerDeposit { get; set; }
    }

   
    public class NetPayPaymentRequestModel
    { 
        public string MerchantUniqueId { get; set; }         
        public string ReturnUrl { get; set; } 
        public decimal Amount { get; set; } 
        public string CustomerName { get; set; } 
        public string Description { get; set; }         
        public string TransactionReference { get; set; }
        public string Currency { get; set; }
        /// <summary>
        /// This is the HMACSHA256 Encryption of the values: MerchantUniqueId, ReturnUrl, Amount, Description, TransactionReference, PaymentType and Currency arranged in AlphabeticalOrder
        /// i.e Amount, Currency, Description, MerchantUniqueId,PaymentType,TransactionReference + 
        /// </summary>
        public string HMAC { get; set; }

        public string FormUrl { get; set; }

    }


    public class TempPaymentRequest
    {
        public string CustomerName { get; set; }
        public decimal Amount { get; set;}
        public string TransactionReference { get; set; }
        /// <summary>
        /// Merchant’s own Transaction ID generated uniquely for each transaction. Your requestReference must be prefixed with a 4 digit prefix provided by Interswitch
        /// </summary>
        public string MerchantRequestReference { get; set; }
        public string Description { get; set; }
        public string PhoneNumber { get; set; }
        public string QuickTellerRedirectUrl { get; set; }
        public string PaymentCode { get; set; }
    }


}
