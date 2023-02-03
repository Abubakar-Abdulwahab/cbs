using Parkway.CBS.Core.HelperModels;
using System;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.HelperModels
{
    public class CharacterCertificateDetailsUpdateVM
    {
        public long CharacterCertificateDetailsId { get; set; }

        public string PassportNumber { get; set; }

        public int DestinationCountryId { get; set; }

        public string DestinationCountry { get; set; }

        public string CustomerName { get; set; }

        public string CountryOfPassport { get; set; }

        public string ReasonForInquiry { get; set; }

        public string PlaceOfBirth { get; set; }

        public string DateOfBirth { get; set; }

        public string FileNumber { get; set; }

        public int CountryOfPassportId { get; set; }

        public DateTime? DateOfIssuance { get; set; }

        public string DateOfIssuanceString { get; set; }

        public string PlaceOfIssuance { get; set; }

        public IEnumerable<CountryVM> Countries { get; set; }
    }
}