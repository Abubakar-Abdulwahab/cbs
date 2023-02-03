using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.TIN.ViewModels
{
    public class TINFormViewModel
    {
        public string ApplicantName { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public AddressViewModel ResidentialAddress { get; set; }

        public string PreviousTaxPayerNumber { get; set; }
        public string IssuingAuthority { get; set; }
        public string IdentificationNumber { get; set; }
        public DateTime? IssuanceDate { get; set; }
        public DateTime? ExpiryDate { get; set; }

        public string PlaceOfIssuance { get; set; }
        public string IssuingAuthorityIdentification { get; set; }
        public DateTime? LastAssessmentDate { get; set; }
        public decimal LastAssessmentAmount { get; set; }
        public DateTime? LastPaymentDate { get; set; }
        public decimal LastPaymentAmount { get; set; }
        public Core.Models.Enums.TaxType TaxType { get; set; }

        public string TaxRepresentativeName { get; set; }
        public string TaxRepresentativeTin { get; set; }
        public string ReasonForRepresentation { get; set; }
        public Core.Models.Enums.RepresentativeType RepType { get; set; }
        public string RepresentativeState { get; set; }
        public AddressViewModel RepresentativeAddress { get; set; }
        public string RepresentativePhoneNumber1 { get; set; }
        public string RepresentativePhoneNumber2 { get; set; }
        public string RepresentativeEmail { get; set; }
        public Core.Models.Enums.SourceOfIncome SourceOfIncome { get; set; }

        public string EmployerName { get; set; }
        public string EmployerTIN { get; set; }
        public DateTime? EmploymentDate { get; set; }

        public string BusinessName { get; set; }
        public DateTime? BusinessCommencementDate { get; set; }
        public AddressViewModel BusinessAddress { get; set; }
        public string BusinessSector { get; set; }
        public string LineOfBusiness { get; set; }
        public int NumberOfEmployees { get; set; }

        public string OwnershipName { get; set; }
        public string CompanyTIN { get; set; }
        public DateTime? OwnershipStartDate { get; set; }

        public virtual AssetViewModel AssetOwned { get; set; }
        public int SharePercentage { get; set; }
        public string SpouseSurname { get; set; }
        public string SpouseFirstName { get; set; }
        public string SpouseMiddleName { get; set; }
        public string SpouseTIN { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public string Signature { get; set; }

        public string DepChild1FirstName { get; set; }
        public string DepChildLastName { get; set; }
        public string DepChildMiddleName { get; set; }
        public string DepChildState { get; set; }
        public string DepChildRelationship { get; set; }
        public string DepChildTIN { get; set; }
        public DateTime? DepChildDateofBirth { get; set; }

        public string DepChild1FirstName1 { get; set; }
        public string DepChildLastName1 { get; set; }
        public string DepChildMiddleName1 { get; set; }
        public string DepChildState1 { get; set; }
        public string DepChildRelationship1 { get; set; }
        public string DepChildTIN1 { get; set; }
        public DateTime? DepChildDateofBirth1 { get; set; }

        public string DepChild1FirstName2 { get; set; }
        public string DepChildLastName2 { get; set; }
        public string DepChildMiddleName2 { get; set; }
        public string DepChildState2 { get; set; }
        public string DepChildRelationship2 { get; set; }
        public string DepChildTIN2 { get; set; }
        public DateTime? DepChildDateofBirth2 { get; set; }

        public bool Attestation { get; set; }

        public string AssetsTable { get; set; }
        public string DependantsTable { get; set; }

        public IList<dynamic> SectorsList { get; set; }
        public IList<dynamic> LGAList { get; set; }
        public IList<dynamic> LocalGovernmentList { get; set; }
        public ApplicantViewModel Applicant { get; set; }
        public Core.HelperModels.HeaderObj HeaderObj { get; set; }
        public string Sector { get; set; }

        public TINFormViewModel()
        {

        }

    }
}