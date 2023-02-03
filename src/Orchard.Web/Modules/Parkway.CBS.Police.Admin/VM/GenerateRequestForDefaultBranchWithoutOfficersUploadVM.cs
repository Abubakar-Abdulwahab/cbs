using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.DTO;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Admin.VM
{
    public class GenerateRequestForDefaultBranchWithoutOfficersUploadVM
    {
        public TaxEntityProfileLocationVM BranchDetails { get; set; }

        public IEnumerable<GenerateRequestWithoutOfficersUploadBatchStagingDTO> GenerateRequestWithoutOfficersUploadBatches { get; set; }

        public int TotalRecordCount { get; set; }

        public dynamic Pager { get; set; }
    }
}