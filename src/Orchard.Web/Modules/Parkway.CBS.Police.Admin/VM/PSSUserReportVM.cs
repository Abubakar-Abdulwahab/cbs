using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Admin.VM
{
    public class PSSUserReportVM
    {
        public List<PSSUserReportCBSUserVM> CBSUsers { get; set; }

        public string Name { get; set; }

        public string UserName { get; set; }

        public string IdentificationNumber { get; set; }

        public dynamic Pager { get; set; }

        public int TotalNumberOfUsers { get; set; }

        public string From { get; set; }

        public string End { get; set; }
    }
}