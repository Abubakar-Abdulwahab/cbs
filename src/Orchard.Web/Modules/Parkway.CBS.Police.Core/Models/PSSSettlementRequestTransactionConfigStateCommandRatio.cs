using Parkway.CBS.Core.Models;


namespace Parkway.CBS.Police.Core.Models
{
    public class PSSSettlementRequestTransactionConfigStateCommandRatio : CBSBaseModel
    {
        public virtual long Id { get; set; }

        public virtual PSSServiceSettlementConfigurationTransaction ConfigTransaction { get; set; }

        public virtual TransactionLog TransactionLog { get; set; }

        public virtual Command StateCommand { get; set; }

        public virtual StateModel State { get; set; }

        public virtual LGA LGA { get; set; }

        public virtual PSSSettlementBatch Batch { get; set; }

        public virtual PSSRequest Request { get; set; }

        public virtual int StateCommandRatio { get; set; }

        public virtual bool FallRatioFlag { get; set; }
    }
    
}