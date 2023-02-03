using CBSPay.Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBSPay.Core.Entities
{
    public class AssessmentRuleItem : BaseEntity<long>
    {
        public virtual long AAIID { get; set; }
        public virtual  long AARID { get; set; }
        public virtual  long AssessmentRuleID { get; set; }
        public virtual  string AssessmentRuleName { get; set; }
        public virtual  string AssessmentItemReferenceNo { get; set; }
        public virtual  long AssessmentItemID { get; set; }
        public virtual  string AssessmentItemName { get; set; }
        public virtual  string ComputationName { get; set; }
        public virtual  long PaymentStatusID { get; set; }
        public virtual  string PaymentStatusName { get; set; }
        public virtual  decimal TaxAmount { get; set; }
        public virtual  decimal AmountPaid { get; set; }
        public virtual  decimal SettlementAmount { get; set; }
        public virtual  decimal PendingAmount { get; set; }
        [JsonIgnore]
        public virtual AssessmentDetailsResult AssessmentDetails { get; set; }

        protected internal virtual void CopyFrom(AssessmentRuleItem item)
        {
            AAIID = item.AAIID;
            this.AARID = item.AARID;
            this.PendingAmount = item.PendingAmount;
            this.SettlementAmount = item.SettlementAmount;
            this.TaxAmount = item.TaxAmount;
            this.DateModified = DateTime.Now;
            this.PaymentStatusID = item.PaymentStatusID;
            this.PaymentStatusName = item.PaymentStatusName;
            this.ComputationName = item.ComputationName;
            this.AssessmentItemName = item.AssessmentItemName;
            this.AssessmentRuleID = item.AssessmentRuleID;
            this.AssessmentRuleName = item.AssessmentRuleName;
            this.AssessmentItemID = item.AssessmentItemID;
            this.AssessmentItemReferenceNo = item.AssessmentItemReferenceNo;

        }
    }
}
