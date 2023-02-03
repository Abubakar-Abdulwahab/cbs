using System;
using Orchard.Users.Models;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    public abstract class PSSCharacterCertificateDetailsBase : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual PSSRequest Request { get; set; }

        public virtual CharacterCertificateReasonForInquiry Reason { get; set; }

        public virtual string ReasonValue { get; set; }

        public virtual Ethnicity Tribe { get; set; }

        public virtual string TribeValue { get; set; }

        public virtual StateModel StateOfOrigin { get; set; }

        public virtual string StateOfOriginValue { get; set; }

        public virtual string PlaceOfBirth { get; set; }

        public virtual Country DestinationCountry { get; set; }

        public virtual string DestinationCountryValue { get; set; }

        public virtual bool PreviouslyConvicted { get; set; }

        public virtual bool IsBiometricEnrolled { get; set; }

        public virtual string PreviousConvictionHistory { get; set; }

        public virtual string PassportNumber { get; set; }

        public virtual string PlaceOfIssuance { get; set; }

        public virtual DateTime? DateOfIssuance { get; set; }

        public virtual string RefNumber { get; set; }

        public virtual bool IsApplicantInvitedForCapture { get; set; }

        public virtual DateTime? CaptureInvitationDate { get; set; }

        public virtual PSSCharacterCertificateRequestType RequestType { get; set; }

        public virtual DateTime? DateOfBirth { get; set; }

        public virtual DateTime? BiometricCaptureDueDate { get; set; }

        public virtual string CPCCRServiceNumber { get; set; }

        public virtual string CPCCRName { get; set; }

        public virtual UserPartRecord CPCCRAddedBy { get; set; }

        public virtual string CPCCRRankCode { get; set; }

        public virtual string CPCCRRankName { get; set; }

        public virtual Country CountryOfOrigin { get; set; }

        public virtual string CountryOfOriginValue { get; set; }

        public virtual Country CountryOfPassport { get; set; }

        public virtual string CountryOfPassportValue { get; set; }
    }
}