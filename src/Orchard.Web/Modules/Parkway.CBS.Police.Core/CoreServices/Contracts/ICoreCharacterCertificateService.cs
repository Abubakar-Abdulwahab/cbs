using Orchard;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.VM;
using System;

namespace Parkway.CBS.Police.Core.CoreServices.Contracts
{
    public interface ICoreCharacterCertificateService : IDependency
    {
        /// <summary>
        /// Checks if there is a character certificate request with specified file ref number that has been approved
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <returns></returns>
        bool CheckIfApprovedCharacterCertificateRequestExists(string fileRefNumber);

        /// <summary>
        /// Checks if there is a character certificate request with specified file ref number that has been approved for the currently logged in user
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <returns></returns>
        bool CheckIfApprovedCharacterCertificateRequestExists(string fileRefNumber, long taxEntityId);

        /// <summary>
        /// Checks if there is a character certificate request with specified file ref number that has been rejected
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <param name="taxEntityId"></param>
        /// <returns></returns>
        bool CheckIfRejectedCharacterCertificateRequestExists(string fileRefNumber, long taxEntityId);

        /// <summary>
        /// Checks if there is a character certificate request with specified file ref number that has been rejected
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <returns></returns>
        bool CheckIfRejectedCharacterCertificateRequestExists(string fileRefNumber);

        /// <summary>
        /// Check if this definition level will be the one to enter the reference number. 
        /// This returns the next approval button name
        /// </summary>
        /// <param name="definitionId"></param>
        /// <param name="position"></param>
        /// <returns>string</returns>
        string CheckIfCanShowRefNumberForm(int definitionId, int position);

        /// <summary>
        /// Checks if the character certificate with the specified approval number has a signature attached
        /// </summary>
        /// <param name="approvalNumber"></param>
        /// <returns></returns>
        bool CheckIfCharacterCertificateIsSigned(string approvalNumber);

        /// <summary>
        /// Generates and saves character certificate for request with specified file ref number
        /// </summary>
        /// <param name="fileRefNumber"></param>
        void CreateAndSaveCertificateDocument(string fileRefNumber);

        /// <summary>
        /// Generates and saves rejection character certificate for request with specified <paramref name="fileRefNumber"/>
        /// </summary>
        /// <param name="fileRefNumber"></param>
        void CreateAndSaveRejectionCertificateDocument(string fileRefNumber);

        /// <summary>
        /// Retrieves PSS Character Certificate
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <param name="returnByte"></param>
        /// <returns></returns>
        CreateCertificateDocumentVM CreateCertificateDocument(string fileRefNumber, bool returnByte = false);

        /// <summary>
        /// Retrieves rejection character certificate
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <param name="returnByte"></param>
        /// <returns></returns>
        CreateCertificateDocumentVM CreateRejectionCertificateDocument(string fileRefNumber, bool returnByte = false);

        /// <summary>
        /// Retrieves Default PSS Character Certificate before approval
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <param name="returnByte"></param>
        /// <returns></returns>
        CreateCertificateDocumentVM CreateDefaultCertificateDocument(string fileRefNumber);

        /// <summary>
        /// Get biometric invitation details
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns>PSSRequestDetailsVM</returns>
        PSSRequestDetailsVM GetBiometricInvitationDetails(long requestId);

        /// <summary>
        /// Gets Character Certificate Biometrics by <paramref name="requestId"/>
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns>CharacterCertificateBiometricsVM</returns>
        CharacterCertificateBiometricsVM GetCharacterCertificateBiometric(long requestId);

        /// <summary>
        /// Update an applicant biometric invitation date
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="biometricCaptureDueDate"></param>
        void UpdateApplicantBiometricInvitationDetails(long requestId, DateTime biometricCaptureDueDate);



        bool ChangePassportPhoto(string filePathName, CharacterCertificateDocumentVM certDeets);
    }
}
