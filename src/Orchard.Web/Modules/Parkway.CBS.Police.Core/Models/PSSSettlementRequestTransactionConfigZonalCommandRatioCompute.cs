using Parkway.CBS.Core.Models;


namespace Parkway.CBS.Police.Core.Models
{
    public class PSSSettlementRequestTransactionConfigZonalCommandRatioCompute : CBSBaseModel
    {
        public virtual long Id { get; set; }

        public virtual PSSServiceSettlementConfigurationTransaction ConfigTransaction { get; set; }

        public virtual TransactionLog TransactionLog { get; set; }

        public virtual Command ZonalCommand { get; set; }

        public virtual PSSSettlementBatch Batch { get; set; }

        public virtual PSSRequest Request { get; set; }

        public virtual int ZonalCommandRatio { get; set; }

        public virtual int ZonalCommandRatioSum { get; set; }

        public virtual decimal RatioAmount { get; set; }

        public virtual bool FallRatioFlag { get; set; }

        public virtual decimal FeePercentage { get; set; }

        public virtual PSSSettlementFeeParty FeeParty { get; set; }

        public virtual string FeePartyName { get; set; }

        public virtual string FeePartyAccountNumber { get; set; }

        public virtual string FeePartyBankCodeForAccountNumber { get; set; }

        public virtual CommandWalletDetails CommandWalletDetails { get; set; }

        public virtual StateModel State { get; set; }

        public virtual LGA LGA { get; set; }
    }
}