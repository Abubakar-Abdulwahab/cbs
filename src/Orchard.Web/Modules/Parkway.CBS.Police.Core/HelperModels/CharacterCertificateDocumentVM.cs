using System;

namespace Parkway.CBS.Police.Core.HelperModels
{
    public class CharacterCertificateDocumentVM
    {
        public long CharacterCertificateDetailsId { get; set; }

        public long RequestId { get; set; }

        public string ApprovalNumber { get; set; }

        public string PassportPhotoBlob { get; set; }

        public string PassportPhotoContentType { get; set; }

        public string RefNumber { get; set; }

        public DateTime DateOfApproval { get; set; }

        public string DateOfApprovalString => DateOfApproval.ToString("dd/MM/yyyy");

        public DateTime DateOfRejection { get; set; }

        public string DateOfRejectionString => DateOfRejection.ToString("dd/MM/yyyy");

        public string CustomerName { get; set; }

        public string CountryOfPassport { get; set; }

        public string PassportNumber { get; set; }

        public string PlaceOfIssuance { get; set; }

        public DateTime? DateOfIssuance { get; set; }

        public string DateOfIssuanceString => (DateOfIssuance != null) ? DateOfIssuance.Value.ToString("dd/MM/yyyy") : "";

        public string ReasonForInquiry { get; set; }

        public string DestinationCountry { get; set; }

        public string CountryOfOrigin { get; set; }

        public string StateOfOrigin { get; set; }

        public string Tribe { get; set; }

        public string PlaceOfBirth { get; set; }

        public string DateOfBirth { get; set; }

        public string PoliceCentralRegistrarName { get; set; }

        public string LogoURL { get; set; }

        public string PSSCharacterCertificateBGPath { get; set; }

        public string PSSCertificateStripURL { get; set; }

        public string PassportPhotoImageSrc { get; set; }

        public string Template { get; set; }

        public bool PreviouslyConvicted { get; set; }

        public bool IsBiometricsEnrolled { get; set; }

        public int FlowDefinitionLevelId { get; set; }

        /// <summary>
        /// Tracks if an applicant has been invited for biometric capture
        /// </summary>
        public bool HasApplicantBeenInvitedForCapture { get; set; }

        public int ServiceTypeId { get; set; }

        public DateTime? BiometricCaptureDueDate { get; set; }

        public string PossapLogoUrl { get; set; }

        public string PccEStampUrl { get; set; }

        public string ValidateDocumentUrl { get; set; }

        public string RequestType { get; set; }

        public string CPCCRName { get; set; }

        public string CPCCRRankCode { get; set; }

        public string CPCCRRankName { get; set; }

        public string CPCCRSignatureBlob { get; set; }

        public string CPCCRSignatureContentType { get; set; }

        public string CPCCRSignatureImageSrc { get; set; }
    }
}