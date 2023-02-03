using CBSPay.Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBSPay.Core.Entities
{
    public class ServiceBillItem : BaseEntity<long>
    {
        public virtual long SBSIID { get; set; }
        public virtual long SBSID { get; set; }
        public virtual long MDAServiceID { get; set; }
        public virtual string MDAServiceName { get; set; }
        public virtual string MDAServiceItemReferenceNo { get; set; }
        public virtual long MDAServiceItemID { get; set; }
        public virtual string MDAServiceItemName { get; set; }
        public virtual string ComputationName { get; set; }
        public virtual long PaymentStatusID { get; set; }
        public virtual string PaymentStatusName { get; set; }
        public virtual decimal? ServiceAmount { get; set; }
        public virtual decimal? AmountPaid { get; set; }
        public virtual decimal? SettlementAmount { get; set; }
        public virtual decimal? PendingAmount { get; set; }
        [JsonIgnore]
        public virtual ServiceBillResult ServiceBill { get; set; }

        protected internal virtual void CopyFrom(ServiceBillItem item)
        {
            this.PaymentStatusID = item.PaymentStatusID;
            this.PaymentStatusName = item.PaymentStatusName;
            this.PendingAmount = item.PendingAmount;
            this.ServiceAmount = item.ServiceAmount;
            this.SettlementAmount = item.SettlementAmount;
            this.DateModified = DateTime.Now;
            MDAServiceName = item.MDAServiceName;
            ComputationName = item.ComputationName;
            MDAServiceItemName = item.MDAServiceItemName;
            SettlementAmount = item.SettlementAmount;
            this.SBSID = item.SBSID;
            this.SBSIID = item.SBSIID;
            this.MDAServiceID = item.MDAServiceID;
            this.MDAServiceItemReferenceNo = item.MDAServiceItemReferenceNo;
            this.MDAServiceItemID = item.MDAServiceItemID;
        }
    }
}
