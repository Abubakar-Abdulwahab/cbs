using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Module.ViewModels
{
    public class PaymentNotificationModel
    {
        /// <summary>
        /// 00 represents success, any other number means not successful
        /// </summary>
        public string ResponseCode { get; set; }
        public string ResponseDesc { get; set; }
        public string MerchantTransactionReference { get; set; }
        public string PaymentReference { get; set; }
        public string HMAC { get; set; }
        public decimal TransactionAmount { get; set; }
    }
}