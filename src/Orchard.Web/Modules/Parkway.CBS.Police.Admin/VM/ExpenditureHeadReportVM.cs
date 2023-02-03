using System.Collections.Generic;

namespace Parkway.CBS.Police.Admin.VM
{
    public class ExpenditureHeadReportVM
    {
        public string ExpenditureHeadName { get; set; }

        public string Code { get; set; }

        public int Status { get; set; }

        public IEnumerable<Core.HelperModels.ExpenditureHeadReportVM> ExpenditureHeadReports { get; set; }

        public dynamic Pager { get; set; }

        public int TotalExpenditureHeadRecord { get; set; }
    }
}