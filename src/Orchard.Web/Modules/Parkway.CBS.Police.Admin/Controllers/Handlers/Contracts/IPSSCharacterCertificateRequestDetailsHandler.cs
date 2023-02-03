using Orchard;
using Orchard.Security.Permissions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts
{
    public interface IPSSCharacterCertificateRequestDetailsHandler : IDependency
    {
        /// <summary>
        /// Generate character certificate document
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <returns></returns>
        CreateCertificateDocumentVM CreateCharacterCertificateByteFile(string fileRefNumber);

        /// <summary>
        /// Invite an applicant for biometric capture
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns>string</returns>
        string InviteApplicantForBiometricCapture(long requestId);

        /// <summary>
        /// Get character certification biometerics
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns>CharacterCertificateBiometricsVM</returns>
        CharacterCertificateBiometricsVM GetCharacterCertificateBiometrics(long requestId);

        /// <summary>
        /// Check for user permission
        /// </summary>
        /// <param name="canViewRequests"></param>
        void CheckForPermission(Permission canViewRequests);

        /// <summary>
        /// Creates and saves the rejection certificate
        /// </summary>
        /// <param name="characterCertificateDetail"></param>
        /// <param name="errors"></param>
        void CreateAndSaveRejectionCertificate(CharacterCertificateRequestDetailsVM characterCertificateDetail, ref List<ErrorModel> errors);
    }
}
