using System;
using System.Collections.Generic;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.HelperModels;

namespace Parkway.CBS.Police.Core.VM
{
    public class CharacterCertificateRequestVM : RequestDumpVM
    {
        public List<CommandVM> ListOfCommands { get; set; }

        public int CharacterCertificateReasonForInquiry { get; set; }

        public string ReasonForInquiryValue { get; set; }

        public IEnumerable<CharacterCertificateReasonForInquiryVM> CharacterCertificateReasonsForInquiry { get; set; }

        public bool ShowReasonForInquiryFreeForm { get; set; }

        public int SelectedCountryOfOrigin { get; set; }
        
        public string SelectedCountryOfOriginValue { get; set; }

        public int SelectedStateOfOrigin { get; set; }

        public string SelectedStateOfOriginValue { get; set; }

        public IEnumerable<EthnicityVM> Tribes { get; set; }

        public string PlaceOfBirth { get; set; }

        public string DateOfBirth { get; set; }

        public DateTime? DateOfBirthParsed { get; set; }

        public int DestinationCountry { get; set; }

        public int SelectedCountryOfPassport { get; set; }

        public string SelectedCountryOfPassportValue { get; set; }

        public string PassportNumber { get; set; }

        public string PlaceOfIssuance { get; set; }

        public string DateOfIssuance { get; set; }
        
        public DateTime? DateOfIssuanceParsed { get; set; }

        public string DestinationCountryValue { get; set; }

        public IEnumerable<CountryVM> Countries { get; set; }

        public string PassportPhotographUploadName { get; set; }

        public string InternationalPassportDataPageUploadName { get; set; }

        public string SignatureUploadName { get; set; }

        public string PassportPhotographUploadPath { get; set; }

        public string InternationalPassportDataPageUploadPath { get; set; }

        public string SignatureUploadPath { get; set; }

        public bool PreviouslyConvicted { get; set; }

        public string PreviousConvictionHistory { get; set; }

        public bool ShowConvictionFreeForm { get; set; }

        public bool ViewedTermsAndConditionsModal { get; set; }

        public PSServiceCaveatVM Caveat { get; set; }

        public int RequestType { get; set; }

        public string RequestTypeValue { get; set; }

        public IEnumerable<CharacterCertificateRequestTypeVM> RequestTypes { get; set; }
    }
}