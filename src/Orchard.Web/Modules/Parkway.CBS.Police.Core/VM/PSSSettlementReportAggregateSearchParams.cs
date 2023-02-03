using System;

namespace Parkway.CBS.Police.Core.VM
{
    public class PSSSettlementReportAggregateSearchParams
    {
        public int Skip { get; set; }

        public int Take { get; set; }

        public bool PageData { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int SettlementId { get; set; }

        public int Status { get; set; }
    }
}