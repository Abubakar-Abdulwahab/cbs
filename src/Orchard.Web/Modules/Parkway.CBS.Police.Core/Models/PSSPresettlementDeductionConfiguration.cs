using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    public class PSSPresettlementDeductionConfiguration : CBSModel
    {
        public virtual string Name { get; set; }

        public virtual SettlementRule SettlementRule { get; set; }

        public virtual string ImplementClass { get; set; }

        public virtual PSService Service { get; set; }

        public virtual MDA MDA { get; set; }

        public virtual RevenueHead RevenueHead { get; set; }

        public virtual ExternalPaymentProvider PaymentProvider { get; set; }

        /// <summary>
        /// core enum type <see cref="CBS.Core.Models.Enums.PaymentChannel"/>
        /// </summary>
        public virtual int Channel { get; set; }

        public virtual PSServiceRequestFlowDefinitionLevel DefinitionLevel { get; set; }

        /// <summary>
        /// core enum type <see cref="Enums.DeductionShareType"/>
        /// </summary>
        public virtual int DeductionShareTypeId { get; set; }

        public virtual decimal PercentageShare { get; set; }

        public virtual decimal FlatShare { get; set; }
    }
}