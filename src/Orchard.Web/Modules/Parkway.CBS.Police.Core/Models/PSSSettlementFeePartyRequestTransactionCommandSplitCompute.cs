using System;
using Parkway.CBS.Core.Models;


namespace Parkway.CBS.Police.Core.Models
{
    public class PSSSettlementFeePartyRequestTransactionCommandSplitCompute : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual PSSSettlementFeePartyRequestTransaction FeePartyRequestTransaction { get; set; }

        public virtual bool FallFlag { get; set; }

        public virtual PSSSettlementBatch Batch { get; set; }

        public virtual PSSSettlementFeeParty SettlementFeeParty { get; set; }

        public virtual PSSFeeParty FeeParty { get; set; }

        public virtual PSSServiceSettlementConfigurationTransaction ConfigTransaction { get; set; }

        public virtual decimal AmountToSplit { get; set; }

        public virtual decimal SplitItemCount { get; set; }

        public virtual decimal SplitPercentage { get; set; }

        public virtual decimal SplitAmount { get; set; }

        public virtual Command Command { get; set; }

        public virtual Command StateCommand { get; set; }

        public virtual Command ZonalCommand { get; set; }

        public virtual StateModel State { get; set; }

        public virtual LGA LGA { get; set; }

        public virtual RequestCommand RequestCommand { get; set; }

        public virtual bool HasAdditionalSplit { get; set; }

        public virtual string AdditionalSplitValue { get; set; }

        public virtual Command SettlementCommand { get; set; }

        /// <summary>
        /// <see cref="Enums.SettlementAccountType"/>
        /// </summary>
        public virtual int SettlementAccountType { get; set; }

    }
}