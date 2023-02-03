using System;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.VM
{
    public class PSSSettlementReportBatchBreakdownVM
    {
        public IEnumerable<PSSSettlementBatchItemsVM> ReportRecords { get; set; }

        public dynamic Pager { get; set; }

        public string SettlementName { get; set; }

        public decimal AmountSettled { get; set; }

        public DateTime SettlementStartDate { get; set; }

        public DateTime SettlementEndDate { get; set; }

        public int TotalRecordCount { get; set; }

        public DateTime SettlementDate { get; set; }
    }
}