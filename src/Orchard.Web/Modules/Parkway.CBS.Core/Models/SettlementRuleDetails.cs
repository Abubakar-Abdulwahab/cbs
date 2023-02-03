using System;

namespace Parkway.CBS.Core.Models
{
    public class SettlementRuleDetails : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual SettlementRule SettlementRule { get; set; }

        public virtual MDA MDA { get; set; }

        public virtual RevenueHead RevenueHead { get; set; }

        public virtual ExternalPaymentProvider PaymentProvider { get; set; }

        /// <summary>
        /// <see cref="Enums.PaymentChannel"/>
        /// </summary>
        public virtual int PaymentChannel_Id { get; set; }


        public virtual bool IsDeleted { get; set; }

    }
}