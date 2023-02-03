using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Models
{
    public class PSSSettlementPercentageRecalculationFeePartyBatchAggregate : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual PSSSettlementBatch Batch { get; set; }

        public virtual PSSSettlementFeeParty SettlementFeeParty { get; set; }

        public virtual PSSFeeParty FeeParty { get; set; }

        public virtual decimal TotalSettlementAmount { get; set; }

        public virtual decimal Percentage { get; set; }

        public virtual decimal CommandPercentage { get; set; }

        public virtual bool FallFlag { get; set; }

        public virtual decimal AggregateTotalSettlementAmount { get; set; }

        public virtual string FeePartyName { get; set; }

        public virtual string BankName { get; set; }

        public virtual string BankCode { get; set; }

        public virtual string BankAccountNumber { get; set; }

        public virtual Command Command { get; set; }

        public virtual string AdditionalSplitValue { get; set; }
    }
}
