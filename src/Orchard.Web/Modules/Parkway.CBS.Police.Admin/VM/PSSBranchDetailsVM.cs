using Parkway.CBS.Core.HelperModels;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Admin.VM
{
    public class PSSBranchDetailsVM
    {
        public TaxEntityViewModel EntityDetails { get; set; }

        public IEnumerable<TaxEntityProfileLocationVM> Branches { get; set; }

        public int TotalRecordCount { get; set; }

        public dynamic Pager { get; set; }
    }
}