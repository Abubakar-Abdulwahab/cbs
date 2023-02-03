using System;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.VM
{
    public class PSSSettlementReportSummaryVM
    {
        public string From { get; set; }

        public string End { get; set; }

        public IEnumerable<PSSSettlementBatchAggregateVM> ReportRecords { get; set; }

        public int TotalReportRecords { get; set; }

        public dynamic Pager { get; set; }

        public string LogoURL { get; set; }

        public string TenantName { get; set; }

        public decimal TotalReportAmount { get; set; }

        public string SettlementName { get; set; }

    }
}