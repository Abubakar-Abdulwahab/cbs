using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Models
{
    public class TINRegistrationForm : CBSBaseModel
    {
        public virtual long Id { get; set; }
        public virtual Applicant Applicant { get; set; }
        public virtual Address ResidentialAddress { get; set; }
        public virtual Address MailingAddress { get; set; }
        public virtual string PreviousTaxPayerNumber { get; set; }
        public virtual string IssuingAuthority { get; set; }
        public virtual string IdentificationNumber { get; set; }
        public virtual DateTime IssuanceDate { get; set; }
        public virtual DateTime ExpiryDate { get; set; }
        //true up
        public virtual string TINApplicantReference { get; set; }

        public virtual string PlaceOfIssuance { get; set; }
        public virtual string IssuingAuthorityIdentification { get; set; }
        public virtual DateTime LastAssessmentDate { get; set; }
        public virtual decimal LastAssessmentAmount { get; set; }
        public virtual DateTime LastPaymentDate { get; set; }
        public virtual Enums.TaxType TaxType { get; set; }

        public virtual string TaxRepresentativeName { get; set; }
        public virtual string TaxRepresentativeTin { get; set; }
        public virtual string ReasonForRepresentation { get; set; }
        public virtual Enums.RepresentativeType RepType { get; set; }
        public virtual string RepresentativeState { get; set; }
        public virtual Address RepresentativeAddress { get; set; }
        public virtual string RepresentativePhoneNumber1 { get; set; }
        public virtual string RepresentativePhoneNumber2 { get; set; }
        public virtual string RepresentativeEmail { get; set; }
        public virtual Enums.SourceOfIncome SourceOfIncome { get; set; }

        //EMPLOYED
        public virtual string EmployerName { get; set; }
        public virtual string EmployerTIN { get; set; }
        public virtual DateTime EmploymentDate { get; set; }

        //SelfEmployed
        public virtual string BusinessName { get; set; }
        public virtual DateTime BusinessCommencementDate { get; set; }
        public virtual Address BusinessAddress { get; set; }
        public virtual string BusinessSector { get; set; }
        public virtual string LineOfBusiness { get; set; }
        public virtual int NumberOfEmployees { get; set; }

        //Partner Owner
        public virtual string OwnershipName { get; set; }

        public virtual string CompanyTIN { get; set; }

        public virtual DateTime OwnershipStartDate { get; set; }

        public virtual int SharePercentage { get; set; }
        public virtual IEnumerable<Asset> AssetOwned { get; set; }
        public virtual string SpouseSurname { get; set; }
        public virtual string SpouseFirstName { get; set; }
        public virtual string SpouseMiddleName { get; set; }
        public virtual string SpouseTIN { get; set; }
        public virtual DateTime StartDate { get; set; }
        public virtual DateTime RegistrationDate { get; set; }
        public virtual string Signature { get; set; }

        public virtual string DepChild1FirstName { get; set; }
        public virtual string DepChildLastName { get; set; }
        public virtual string DepChildMiddleName { get; set; }
        public virtual string DepChildState { get; set; }
        public virtual string DepChildRelationship { get; set; }
        public virtual string DepChildTIN { get; set; }
        public virtual DateTime DepChildDateofBirth { get; set; }

        public virtual string DepChild1FirstName1 { get; set; }
        public virtual string DepChildLastName1 { get; set; }
        public virtual string DepChildMiddleName1 { get; set; }
        public virtual string DepChildState1 { get; set; }
        public virtual string DepChildRelationship1 { get; set; }
        public virtual string DepChildTIN1 { get; set; }
        public virtual DateTime DepChildDateofBirth1 { get; set; }

        public virtual string DepChild1FirstName2 { get; set; }
        public virtual string DepChildLastName2 { get; set; }
        public virtual string DepChildMiddleName2 { get; set; }
        public virtual string DepChildState2 { get; set; }
        public virtual string DepChildRelationship2 { get; set; }
        public virtual string DepChildTIN2 { get; set; }
        public virtual DateTime DepChildDateofBirth2 { get; set; }
        public virtual string TIN { get; set; }

    }
}