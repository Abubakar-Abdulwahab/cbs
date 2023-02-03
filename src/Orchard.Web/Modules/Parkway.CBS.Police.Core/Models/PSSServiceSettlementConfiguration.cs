using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    /// <summary>
    /// this defines the settlement paramters 
    /// </summary>
    public class PSSServiceSettlementConfiguration : CBSModel
    {
        public virtual PSSSettlement Settlement { get; set; }

        public virtual PSService Service { get; set; }

        public virtual MDA MDA { get; set; }

        public virtual RevenueHead RevenueHead { get; set; }

        public virtual ExternalPaymentProvider PaymentProvider { get; set; }

        /// <summary>
        /// core enum type <see cref="CBS.Core.Models.Enums.PaymentChannel"/>
        /// </summary>
        public virtual int Channel { get; set; }

        public virtual bool IsActive { get; set; }

        public virtual string CompositeUniqueValue { get; set; }

    }
}