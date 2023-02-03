using System;

namespace Parkway.CBS.Police.Core.DTO
{
    public class PSSSettlementBatchVM
    {
        public Int64 Id { get; set; }

        public int PSSSettlementId { get; set; }

        public int ServiceId { get; set; }

        public int ServiceType { get; set; }

        public DateTime ScheduleDate { get; set; }

        public DateTime SettlementRangeStartDate { get; set; }

        public DateTime SettlementRangeEndDate { get; set; }

        public DateTime TransactionDate { get; set; }

        public string SettlementName { get; set; }

        public decimal SettlementAmount { get; set; }

        public string SettlementBatchRef { get; set; }

        public int Status { get; set; }

        public string StatusMessage { get; set; }

        public DateTime? SettlementDate { get; set; }
    }
}