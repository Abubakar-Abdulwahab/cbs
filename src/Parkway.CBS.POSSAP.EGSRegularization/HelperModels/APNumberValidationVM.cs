using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.POSSAP.EGSRegularization.HelperModels
{
    public class APNumberValidationVM
    {
        public bool HasError { get; set; }

        public string ErrorMessage { get; set; }

        public Police.Core.ExternalSourceData.HRSystem.ViewModels.PersonnelReportRecord PersonnelReportRecord { get; set; }

        public PolicerOfficerLog PoliceOfficerLogModel { get; set; }

        public long PSSBranchSubUsersUploadBatchItemsStagingId { get; set; }
    }
}
