using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.DTO;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Admin.VM
{
    public class PSSBranchSubUsersUploadValidationResultVM
    {
        public PSSBranchSubUsersUploadBatchStagingDTO BatchDetails { get; set; }

        public IEnumerable<PSSBranchSubUsersUploadBatchItemsStagingDTO> Items { get; set; }

        public FileUploadReport PSSBranchSubUsersUploadBatchItemsReport { get; set; }

        public string BatchToken { get; set; }

        public dynamic Pager { get; set; }
    }
}