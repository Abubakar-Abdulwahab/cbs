using System;

namespace Parkway.CBS.POSSAP.Services.HelperModel
{
    public class PSSDeploymentAllowanceTrackerVM
    {
        public long Id { get; set; }

        public long RequestId { get; set; }

        public long InvoiceId { get; set; }

        public long EscortDetailId { get; set; }

        public DateTime EscortStartDate { get; set; }

        public DateTime EscortEndDate { get; set; }

        public int NumberOfSettlementDone { get; set; }

        public DateTime NextSettlementDate { get; set; }

        public DateTime SettlementCycleStartDate { get; set; }

        public DateTime SettlementCycleEndDate { get; set; }
    }
}
