using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBSPay.Core.Models
{
    public class EIRSPaymentRequestItem : BaseEntity<long>
    {
        /// <summary>
        /// AAIID(e.g 1000) for assessment rule Item and SBSIID( e.g 1011) for service bill Item
        /// </summary>
        public virtual long ItemId { get; set; }
        public virtual EIRSPaymentRequest PaymentRequest{ get; set; }
        public virtual string ItemDescription { get; set; }
        public virtual decimal ItemAmount { get; set; }
        public virtual decimal AmountPaid { get; set; }
        public virtual decimal TotalAmountPaid { get; set; }

        //inner bill ref new details

        public virtual int TaxYear { get; set; }
        public virtual long RefRuleID { get; set; }
        public virtual string RuleName { get; set; }
        public virtual long RuleID { get; set; }
        public virtual decimal RuleAmount { get; set; }
        public virtual decimal SettledAmount { get; set; }
        public virtual decimal OutstandingAmount { get; set; }
        public virtual decimal RuleAmountToPay { get; set; }
        public virtual string RuleItemRef { get; set; }
        public virtual string RuleItemID { get; set; }
        public virtual string RuleItemName { get; set; }
        public virtual string RuleComputation { get; set; }
    }
}
