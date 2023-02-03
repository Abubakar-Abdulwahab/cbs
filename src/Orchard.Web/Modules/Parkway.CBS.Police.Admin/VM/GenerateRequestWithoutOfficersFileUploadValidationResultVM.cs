using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.DTO;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Admin.VM
{
    public class GenerateRequestWithoutOfficersFileUploadValidationResultVM
    {
        public GenerateRequestWithoutOfficersUploadBatchStagingDTO BatchDetails { get; set; }

        public IEnumerable<GenerateRequestWithoutOfficersUploadBatchItemsStagingDTO> Items { get; set; }

        public FileUploadReport BatchItemsReport { get; set; }

        public string BatchToken { get; set; }

        public dynamic Pager { get; set; }
    }
}