using Parkway.CBS.Police.Core.DTO;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.VM
{
    public class PSSSettlementReportPartyVM
    {
        public IEnumerable<PSSSettlementFeePartyBatchAggregateVM> ReportRecords { get; set; }

        public PSSSettlementBatchVM SettlementBatch { get; set; }

        public int TotalRecordCount { get; set; }

        public decimal TotalAmountSettled { get; set; }

        public dynamic Pager { get; set; }
    }
}