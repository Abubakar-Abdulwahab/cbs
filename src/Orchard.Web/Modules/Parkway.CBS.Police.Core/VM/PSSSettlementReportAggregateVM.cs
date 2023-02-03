using Parkway.CBS.Police.Core.DTO;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.VM
{
    public class PSSSettlementReportAggregateVM
    {
        public string From { get; set; }

        public string End { get; set; }

        public IEnumerable<PSSSettlementVM> Settlements { get; set; }

        public int SelectedSettlement { get; set; }

        public IEnumerable<PSSSettlementBatchVM> ReportRecords { get; set; }

        public int TotalRecordCount { get; set; }

        public dynamic Pager { get; set; }
    }
}