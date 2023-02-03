using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Models
{
    public class PSSCharacterCertificateRejection : CBSBaseModel
    {

        public virtual string CharacterCertificateRejectionTemplate { get; set; }

        public virtual string CustomerName { get; set; }

        public virtual DateTime? DateOfIssuance { get; set; }

        public virtual DateTime? DateOfRejection { get; set; }

        public virtual string DestinationCountry { get; set; }

        public virtual Int64 Id { get; set; }

        public virtual string NameOfCentralRegistrar { get; set; }

        public virtual string CountryOfPassport { get; set; }

        public virtual string PassportNumber { get; set; }

        public virtual string PassportPhotoBlob { get; set; }

        public virtual string PassportPhotoContentType { get; set; }

        public virtual string PlaceOfIssuance { get; set; }

        public virtual PSSCharacterCertificateDetails PSSCharacterCertificateDetails { get; set; }

        public virtual string ReasonForInquiry { get; set; }

        public virtual string RefNumber { get; set; }

        public virtual string CPCCRRankCode { get; set; }
    }
}