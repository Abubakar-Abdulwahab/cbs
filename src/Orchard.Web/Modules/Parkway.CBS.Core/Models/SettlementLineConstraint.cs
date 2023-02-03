using System;

namespace Parkway.CBS.Core.Models
{
    public class SettlementLineConstraint : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual SettlementRule Settlement { get; set; }

        public virtual RevenueHead RevenueHead { get; set; }

        public virtual MDA MDA { get; set; }

        /// <summary>
        /// <see cref="Enums.PaymentChannel"/>
        /// </summary>
        public virtual int PaymentChannel { get; set; }

    }
}