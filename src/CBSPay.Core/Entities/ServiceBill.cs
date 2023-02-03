using CBSPay.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBSPay.Core.Models
{
    public class ServiceBill
    {
        public virtual bool Success { get; set; }
        public virtual string Message { get; set; }
        public virtual ServiceBillResult Result { get; set; }
    }

    public class ServiceBillResult : BaseEntity<long>
    {
        public virtual long ServiceBillID { get; set; }
        public virtual string ServiceBillRefNo { get; set; }
        public virtual DateTime? ServiceBillDate { get; set; }
        public virtual long TaxPayerID { get; set; }
        public virtual string TaxPayerName { get; set; }
        public virtual string TaxpayerRIN { get; set; }
        public virtual decimal ServiceBillAmount { get; set; }
        public virtual decimal SetlementAmount { get; set; }
        public virtual long SettlementStatusID { get; set; }
        public virtual DateTime? SettlementDueDate { get; set; }
        public virtual string SettlementStatusName { get; set; }
        public virtual DateTime? SettlementDate { get; set; }
        public virtual bool Active { get; set; }
        public virtual string ActiveText { get; set; }
        public virtual string PhoneNumber { get; set; }
        public virtual IEnumerable<ServiceBillItem> ServiceBillItems { get; set; }
        public virtual IEnumerable<MDAService> MDAServiceRules { get; set; }

    }

    public class NetPayPaymentModel
    {
        public virtual decimal Amount { get; set; }
        public virtual string Currency { get; set; }
        public virtual string CustomerName { get; set; }
        public virtual string Description { get; set; }
        public virtual string HMAC { get; set; }
        public virtual string MerchantUniqueId { get; set; }
        public virtual string TransactionReference { get; set; }
        public virtual string ReturnUrl { get; set; }
    }

    public class NetPayRequestModel
    {
        public virtual decimal Amount { get; set; }
        public virtual string Currency { get; set; }
        public virtual string Description { get; set; }
        public virtual string MerchantUniqueId { get; set; }
        public virtual string TransactionReference { get; set; }
        public virtual string ReturnUrl { get; set; }
        public virtual string MerchantSecretKey { get; set; }
        public virtual string CustomerName { get; set; }
        public virtual string HMAC { get; set; }
    }
}

