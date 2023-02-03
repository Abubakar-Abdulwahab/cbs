using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Models
{
    public class ReferenceDataRecords : CBSBaseModel
    {
        public virtual long Id { get; set; }

        public virtual ReferenceDataBatch ReferenceDataBatch { get; set; }

        public virtual string RowId { get; set; }

        public virtual string Completed { get; set; }

        public virtual string CompletedBy { get; set; }

        public virtual string Started { get; set; }

        public virtual string Received { get; set; }

        public virtual string CompletedAt { get; set; }

        public virtual string Title { get; set; }

        public virtual string Sex { get; set; }

        public virtual string Surname { get; set; }

        public virtual string Firstname { get; set; }

        public virtual string Middlename { get; set; }

        public virtual string Nationality { get; set; }

        public virtual string PhoneNumber1 { get; set; }

        public virtual string PhoneNumber2 { get; set; }

        public virtual string EmailAddress { get; set; }

        public virtual string TIN { get; set; }

        public virtual string HouseNo { get; set; }

        public virtual string StreetName { get; set; }

        public virtual string City { get; set; }

        public virtual string LGA { get; set; }

        public virtual string DbLGAId { get; set; }

        public virtual string EmploymentStatus { get; set; }

        public virtual string EmployerName { get; set; }

        public virtual string EmployerAddress { get; set; }

        public virtual string PropertyType { get; set; }

        public virtual string PropertyStructure { get; set; }

        public virtual string PropertyStructureNumber { get; set; }

        public virtual string PropertyAddress { get; set; }

        public virtual decimal PropertyRentAmount { get; set; }

        public virtual string RentStartDate { get; set; }

        public virtual string RentEndDate { get; set; }

        public virtual string OwnerTitle { get; set; }

        public virtual string OwnerSex { get; set; }

        public virtual string OwnerSurname { get; set; }

        public virtual string OwnerFirstname { get; set; }

        public virtual string OwnerMiddlename { get; set; }

        public virtual string OwnerNationality { get; set; }

        public virtual string OwnerPhoneNumber1 { get; set; }

        public virtual string OwnerPhoneNumber2 { get; set; }

        public virtual string OwnerEmailAddress { get; set; }

        public virtual string OwnerTIN { get; set; }

        public virtual string OwnerEmploymentStatus { get; set; }

        public virtual string OwnerEmployerName { get; set; }

        public virtual string OwnerEmployerAddress { get; set; }

        /// <summary>
        /// This is use to track the type of tax a payer has paid for the year 2019
        /// </summary>
        public virtual string TypeOfTaxPaid { get; set; }

        /// <summary>
        /// This is use to track if tax payer has paid for Withholding tax for the year 2019
        /// </summary>
        public virtual Boolean IsEvidenceProvided { get; set; }

        public virtual Boolean IsTaxPayerLandlord { get; set; }

        public virtual string BusinessName { get; set; }

        public virtual string BusinessRegOffice { get; set; }

        public virtual string CorporateHouseAddress { get; set; }

        public virtual string CorporateStreetName { get; set; }

        public virtual string CorporateCity { get; set; }

        public virtual string CorporateLGA { get; set; }

        public virtual string CorporateOrganizationType { get; set; }

        public virtual string CorporateTin { get; set; }

        public virtual string CorporateContactName { get; set; }

        public virtual string CorporatePhoneNumber { get; set; }

        public virtual string ContactEmail { get; set; }

        public virtual string BusinessCommencement { get; set; }

        public virtual string NumberEmployees { get; set; }

        public virtual string DirectorTitle { get; set; }

        public virtual string DirectorSex { get; set; }

        public virtual string DirectorSurname { get; set; }

        public virtual string DirectorFirstName { get; set; }

        public virtual string DirectorMiddleName { get; set; }

        public virtual string DirectorNationality { get; set; }

        public virtual string DirectorPhone1 { get; set; }

        public virtual string DirectorPhone2 { get; set; }

        public virtual string DirectorEmail { get; set; }

        public virtual string DirectorAddress { get; set; }

        public virtual string DirectorCity { get; set; }

        public virtual string CorporateIncomeYr1 { get; set; }

        public virtual string CorporateIncomeYr2 { get; set; }

        public virtual string CorporateIncomeYr3 { get; set; }

        public virtual string SolProSouIncome { get; set; }

        public virtual string AnnualGross { get; set; }

        public virtual string TaxEntityCategory_Id { get; set; }

        public virtual int SerialNumberId { get; set; }
    }
}