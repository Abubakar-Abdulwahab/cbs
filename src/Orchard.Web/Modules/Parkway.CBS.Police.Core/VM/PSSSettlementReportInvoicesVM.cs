using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.VM
{
    public class PSSSettlementReportInvoicesVM
    {
        public IEnumerable<PSSSettlementBatchItemsVM> ReportRecords { get; set; }

        public long TotalReportRecords { get; set; }

        public decimal TotalReportAmount { get; set; }

        public dynamic Pager { get; set; }

        public string LogoURL { get; set; }

        public string TenantName { get; set; }

        public string SettlementBatchRef { get; set; }
    }
}