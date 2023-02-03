using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.HelperModels;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Admin.VM
{
    public class PSSBranchProfileDetailVM
    {
        public TaxEntityProfileLocationVM BranchTaxEntityProfileLocation { get; set; }

        public TaxEntityProfileLocationVM DefaultBranchTaxEntityProfileLocation { get; set; }

        public IEnumerable<PSSBranchOfficersUploadBatchStagingVM> BranchOfficerBatches { get; set; }

        public int ProfileId { get; set; }

        public int TotalRecordCount { get; set; }

        public dynamic Pager { get; set; }
    }
}