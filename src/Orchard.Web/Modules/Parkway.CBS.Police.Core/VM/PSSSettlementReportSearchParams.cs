using System;

namespace Parkway.CBS.Police.Core.VM
{
    public class PSSSettlementReportSearchParams
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int Take { get; set; }

        public int Skip { get; set; }

        public long BatchId { get; set; }

        public bool PageData { get; set; }
    }
}