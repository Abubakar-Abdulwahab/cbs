using System;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.VM
{
    public class PSSSettlementReportPartiesVM
    {
        public IEnumerable<PSSSettlementBatchItemsVM> ReportRecords { get; set; }

        public dynamic Pager { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }
    }
}