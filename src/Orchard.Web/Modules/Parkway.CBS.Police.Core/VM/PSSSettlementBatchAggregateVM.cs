using System;

namespace Parkway.CBS.Police.Core.VM
{
    public class PSSSettlementBatchAggregateVM
    {
        public DateTime TransactionDate { get; set; }

        public string SettlementName { get; set; }

        public decimal SettlementAmount { get; set; }

        public string SettlementBatchId { get; set; }

        public string SettlementBatchRef { get; set; }
    }
}