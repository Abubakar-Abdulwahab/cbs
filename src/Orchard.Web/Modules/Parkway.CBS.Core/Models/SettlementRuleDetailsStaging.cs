using System;

namespace Parkway.CBS.Core.Models
{
    public class SettlementRuleDetailsStaging : CBSBaseModel
    {
      
        public virtual Int64 Id { get; set; }

        public virtual SettlementRuleStaging SettlementRuleStaging { get; set; }

        public virtual MDA MDA { get; set; }

        public virtual RevenueHead RevenueHead { get; set; }

        public virtual ExternalPaymentProvider PaymentProvider { get; set; }

        /// <summary>
        /// <see cref="Enums.PaymentChannel"/>
        /// </summary>
        public virtual int PaymentChannel_Id { get; set; }

        public virtual bool HasErrors { get; set; }

        public virtual string ErrorMessage { get; set; }

        public virtual bool IsOverride { get; set; }

        public virtual SettlementRuleDetails OverrideSettlementRuleDetails { get; set; }

    }
}