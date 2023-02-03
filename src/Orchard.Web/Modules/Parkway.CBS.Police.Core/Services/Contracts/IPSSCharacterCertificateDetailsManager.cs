using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IPSSCharacterCertificateDetailsManager<PSSCharacterCertificateDetails> : IDependency, IBaseManager<PSSCharacterCertificateDetails>
    {
        /// <summary>
        /// Gets character certificate view details
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <param name="taxEntityId"></param>
        /// <returns></returns>
        IEnumerable<CharacterCertificateDetailsVM> GetCharacterCertificateRequestViewDetails(string fileRefNumber, long taxEntityId);

        /// <summary>
        /// Get character certificate document info
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns>IEnumerable<CharacterCertificateDetailsVM></returns>
        IEnumerable<CharacterCertificateDetailsVM> GetCharacterCertificateDocumentInfo(long requestId);
        /// <summary>
        /// Get request details
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns>CharacterCertificateRequestDetailsVM</returns>
        CharacterCertificateRequestDetailsVM GetRequestDetails(long requestId);

        /// <summary>
        /// Gets character certificate request ref number and workflow definition details using request id
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns>CharacterCertificateRequestDetailsVM</returns>
        CharacterCertificateRequestDetailsVM GetRefNumberAndWorkflowDetails(long requestId);

        /// <summary>
        /// Updates Ref Number for character certificate details with specified request id
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="action"></param>
        /// <param name="details"></param>
        void UpdateCharacterCertificateRefNumber(long requestId, string refNumber);

        /// <summary>
        /// Updates <see cref="PSSCharacterCertificateDetails.IsBiometricEnrolled"/> to true using <paramref name="detailsId"/>
        /// </summary>
        /// <param name="detailsId"></param>
        void UpdateCharacterCertificateIsBiometricsEnrolledStatus(long detailsId);

        /// <summary>
        /// Gets details required for generating character certificate
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <returns></returns>
        CharacterCertificateDocumentVM GetCharacterCertificateDocumentDetails(string fileRefNumber);

        /// <summary>
        /// Gets character certificate details by file number
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <returns></returns>
        CharacterCertificateDocumentVM GetPendingCharacterCertificateDocumentDetails(string fileRefNumber);

        /// <summary>
        /// Gets pending character certificate details by file number excluding the passport blob
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <returns>CharacterCertificateDocumentVM</returns>
        CharacterCertificateDocumentVM GetPendingCharacterCertificateDocumentDetailsWithoutPassport(string fileRefNumber);

        /// <summary>
        /// Check if a reference number for a character certificate has been populated
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <returns>bool</returns>
        bool CheckReferenceNumber(string fileRefNumber);

        /// <summary>
        /// Update an applicant biometric invitation details
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="biometricCaptureDueDate"></param>
        void UpdateApplicantBiometricInviteDetails(long requestId, DateTime biometricCaptureDueDate);

        /// <summary>
        /// Get biometric invitation details
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns>PSSRequestDetailsVM</returns>
        PSSRequestDetailsVM GetBiometricInvitationDetails(long requestId);

        /// <summary>
        /// Updates character certificate details CPCCR Name and Service Number
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="serviceNumber"></param>
        /// <param name="name"></param>
        /// <param name="rankCode"></param>
        /// <param name="rankName"></param>
        /// <param name="adminId"></param>
        void UpdateCharacterCertificateCPCCRNameAndServiceNumber(long requestId, string serviceNumber, string name, string rankCode, string rankName, int adminId);

        /// <summary>
        /// Gets details required for generating character certificate
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <returns>CharacterCertificateDetailsUpdateVM</returns>
        CharacterCertificateDetailsUpdateVM GetCharacterCertificateDetailsForEdit(string fileRefNumber);

        /// <summary>
        /// Update Destination Country, Passport Number and Passport Date of Issuance for character certificate details with specified id
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="action"></param>
        /// <param name="details"></param>
        /// <returns>bool</returns>
        bool UpdateCharacterCertificateDetails(CharacterCertificateDetailsUpdateVM model);

        /// <summary>
        /// Gets character certificate details id with specified file ref number
        /// </summary>
        /// <param name="fileNumber"></param>
        /// <returns></returns>
        long GetCharacterCertificateDetailsIdWithFileNumber(string fileNumber);

    }
}
