using Parkway.CBS.Police.Core.DTO;
using System;

namespace Parkway.CBS.Police.Core.VM
{
    public class PSSSettlementFeePartyBatchAggregateVM
    {
        public Int64 Id { get; set; }

        public DateTime TransactionDate { get; set; }

        public DateTime? SettlementDate { get; set; }

        public int FeePartyId { get; set; }

        public string FeePartyName { get; set; }

        public decimal TotalSettlementAmount { get; set; }

        public decimal Percentage { get; set; }

        public string BankName { get; set; }

        public string BankCode { get; set; }

        public string BankAccountNumber { get; set; }

        public PSSSettlementBatchVM Batch { get; set; }

        public int CommandId { get; set; }
    }
}