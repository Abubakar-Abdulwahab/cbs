using System;

namespace CBSPay.Core.APIModels
{
    /// <summary>
    /// m0del for netpay payment notification
    /// </summary>
    public class NetPayPaymentNotificationModel
    {
        /// <summary>
        /// 00 represents success, any other number means not successfull
        /// </summary>
        public string ResponseCode { get; set; }
        public string ResponseDesc { get; set; }
        public string MerchantTransactionReference { get; set; }
        public string PaymentReference { get; set; }
        public string HMAC { get; set; }
        public decimal TransactionAmount { get; set; }
         
    } 

    public class TaxPayerPaymentNotificationModel
    {

        public string ReferenceNumber { get; set; }
        public string RIN { get; set; }
        public string TIN { get; set; }
        public string PhoneNumber { get; set; } 
        public decimal AmountPaid { get; set; } 
        public string PaymentIdentifier { get; set; }
        public DateTime DatePaid { get; set; }
        public string TaxPayerName { get; set; }
        public string Message { get; set; } 
    }
}
