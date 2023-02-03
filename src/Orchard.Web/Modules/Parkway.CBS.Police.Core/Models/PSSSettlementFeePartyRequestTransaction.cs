using System;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    public class PSSSettlementFeePartyRequestTransaction : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual PSSSettlementBatch Batch { get; set; }

        public virtual PSSSettlementFeeParty SettlementFeeParty { get; set; }

        public virtual PSSFeeParty FeeParty { get; set; }

        public virtual PSSServiceSettlementConfigurationTransaction ConfigTransaction { get; set; }

        public virtual TransactionLog TransactionLog { get; set; }

        public virtual PSSRequest Request { get; set; }

        public virtual decimal DeductioPercentage { get; set; }

        public virtual decimal TransactionAmount { get; set; }

        public virtual decimal AmountToSettle { get; set; }

        public virtual bool IsMaxPercentage { get; set; }

        public virtual bool HasAdditionalSplit { get; set; }

        public virtual string AdditionalSplitValue { get; set; }
    }
}