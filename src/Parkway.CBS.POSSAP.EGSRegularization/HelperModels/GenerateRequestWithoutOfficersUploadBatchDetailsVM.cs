using Parkway.CBS.Police.Core.Models.Enums;

namespace Parkway.CBS.POSSAP.EGSRegularization.HelperModels
{
    public class GenerateRequestWithoutOfficersUploadBatchDetailsVM
    {
        public GenerateRequestWithoutOfficersUploadStatus Status { get; set; }

        public string FilePath { get; set; }
    }
}
