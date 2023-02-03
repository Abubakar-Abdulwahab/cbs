using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    /// <summary>
    /// this defines the settlement config and transactions 
    /// </summary>
    public class PSSServiceSettlementConfigurationTransaction : CBSModel
    {
        public virtual PSSSettlementBatch Batch { get; set; }

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

        public virtual TransactionLog TransactionLog { get; set; }

        public virtual decimal SettlementAmount { get; set; }

        public virtual string CompositeUniqueValue { get; set; }
    }
}