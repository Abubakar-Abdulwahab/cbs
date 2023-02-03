using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    /// <summary>
    /// this defines the settlement config and transactions 
    /// </summary>
    public class PSSSettlementRequestTransactionConfigCommand : CBSBaseModel
    {
        public virtual long Id { get; set; }

        public virtual PSSServiceSettlementConfigurationTransaction ConfigTransaction { get; set; }

        public virtual TransactionLog TransactionLog { get; set; }

        public virtual Command Command { get; set; }

        public virtual PSSSettlementBatch Batch { get; set; }

        public virtual PSSRequest Request { get; set; }

        public virtual RequestCommand RequestCommand { get; set; }

        public virtual StateModel State { get; set; }

        public virtual LGA LGA { get; set; }
    }
}