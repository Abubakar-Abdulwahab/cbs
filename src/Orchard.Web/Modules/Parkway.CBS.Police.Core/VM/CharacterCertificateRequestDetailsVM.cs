using Parkway.CBS.Police.Core.DTO;
using System;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.VM
{
    public class CharacterCertificateRequestDetailsVM : PSSRequestDetailsVM
    {
        public string Tribe { get; set; }

        public string CountryOfOrigin { get; set; }

        public string StateOfOrigin { get; set; }

        public string DateOfBirth { get; set; }

        public string PlaceOfBirth { get; set; }

        public string DestinationCountry { get; set; }

        public string IsPreviouslyConvicted { get; set; }

        public bool IsBiometricsEnrolled { get; set; }

        public string PreviousConvictionHistory { get; set; }

        public string PassportPhotographFileName { get; set; }

        public string InternationalPassportDataPageFileName { get; set; }

        public string SignatureFileName { get; set; }

        public string PassportPhotographFilePath { get; set; }

        public string InternationalPassportDataPageFilePath { get; set; }

        public string SignatureFilePath { get; set; }

        public string PassportPhotographContentType { get; set; }

        public string InternationalPassportDataPageContentType { get; set; }

        public string SignatureContentType { get; set; }

        public string PassportPhotographBlob { get; set; }

        public string InternationalPassportDataPageBlob { get; set; }

        public string SignatureBlob { get; set; }

        public string CountryOfPassport { get; set; }

        public string PassportNumber { get; set; }

        public string PlaceOfIssuance { get; set; }

        public string DateOfIssuance { get; set; }

        public string RefNumber { get; set; }

        /// <summary>
        /// Tracks if the policy reference number form should be display to a particular approval level 
        /// </summary>
        public bool ShowReferenceNumberForm { get; set; }

        public int DefinitionId { get; set; }

        public int Position { get; set; }

        public bool IsLastApprover { get; set; }

        public ICollection<ProposedEscortOffficerVM> SelectedCPCCR { get; set; }

        public IEnumerable<PSServiceRequestFlowDefinitionLevelDTO> RequestStages { get; set; }

        public int SelectedRequestStage { get; set; }
    }
}