using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Core.Models
{
    public class PSSPresettlementDeductions : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual string Name { get; set; }

        public virtual PSSSettlementBatch SettlementBatch { get; set; }

        public virtual PSService Service { get; set; }

        public virtual MDA MDA { get; set; }

        public virtual RevenueHead RevenueHead { get; set; }

        public virtual ExternalPaymentProvider PaymentProvider { get; set; }

        /// <summary>
        /// core enum type <see cref="CBS.Core.Models.Enums.PaymentChannel"/>
        /// </summary>
        public virtual int Channel { get; set; }

        public virtual PSSRequest Request { get; set; }

        public virtual Invoice Invoice { get; set; }

        /// <summary>
        /// Deduction amount value
        /// </summary>
        public virtual decimal Amount { get; set; }

    }
}