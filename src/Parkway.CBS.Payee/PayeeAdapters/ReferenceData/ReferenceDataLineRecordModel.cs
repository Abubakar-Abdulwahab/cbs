using Parkway.CBS.Payee.PayeeAdapters.ReferenceData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Payee.ReferenceDataImplementation
{
    public class ReferenceDataLineRecordModel
    {
        public ReferenceDataStringValue Row_Id { get; internal set; }

        public ReferenceDataStringValue Completed { get; internal set; }

        public ReferenceDataStringValue CompletedBy { get; internal set; }

        public ReferenceDataStringValue Started { get; internal set; }

        public ReferenceDataStringValue Received { get; internal set; }

        public ReferenceDataStringValue CompletedAt { get; internal set; }

        public ReferenceDataStringValue Title { get; internal set; }

        public ReferenceDataStringValue Sex { get; internal set; }

        public ReferenceDataStringValue Surname { get; internal set; }

        public ReferenceDataStringValue Firstname { get; internal set; }

        public ReferenceDataStringValue Middlename { get; internal set; }

        public ReferenceDataStringValue Nationality { get; internal set; }

        public ReferenceDataStringValue PhoneNumber1 { get; internal set; }

        public ReferenceDataStringValue PhoneNumber2 { get; internal set; }

        public ReferenceDataStringValue EmailAddress { get; internal set; }

        public ReferenceDataStringValue TIN { get; internal set; }

        public ReferenceDataStringValue HouseNo { get; internal set; }

        public ReferenceDataStringValue StreetName { get; internal set; }

        public ReferenceDataStringValue City { get; internal set; }

        public ReferenceDataStringValue LGA { get; internal set; }

        public string DbLGAId { get; internal set; }

        public ReferenceDataStringValue EmploymentStatus { get; internal set; }

        public ReferenceDataStringValue EmployerName { get; internal set; }

        public ReferenceDataStringValue EmployerAddress { get; internal set; }

        public ReferenceDataStringValue PropertyType { get; internal set; }

        public ReferenceDataStringValue PropertyStructure { get; internal set; }

        public ReferenceDataStringValue PropertyStructureNumber { get; internal set; }

        public ReferenceDataStringValue PropertyAddress { get; internal set; }

        public decimal PropertyRentAmount { get; internal set; }

        public ReferenceDataStringValue RentStartDate { get; internal set; }

        public ReferenceDataStringValue RentEndDate { get; internal set; }

        public ReferenceDataStringValue OwnerTitle { get; internal set; }

        public ReferenceDataStringValue OwnerSex { get; internal set; }

        public ReferenceDataStringValue OwnerSurname { get; internal set; }

        public ReferenceDataStringValue OwnerFirstname { get; internal set; }

        public ReferenceDataStringValue OwnerMiddlename { get; internal set; }

        public ReferenceDataStringValue OwnerNationality { get; internal set; }

        public ReferenceDataStringValue OwnerPhoneNumber1 { get; internal set; }

        public ReferenceDataStringValue OwnerPhoneNumber2 { get; internal set; }

        public ReferenceDataStringValue OwnerEmailAddress { get; internal set; }

        public ReferenceDataStringValue OwnerTIN { get; internal set; }

        public ReferenceDataStringValue OwnerEmploymentStatus { get; internal set; }

        public ReferenceDataStringValue OwnerEmployerName { get; internal set; }

        public ReferenceDataStringValue OwnerEmployerAddress { get; internal set; }

        public ReferenceDataStringValue TypeOfTaxPaid { get; internal set; }

        public Boolean EvidenceProvided { get; internal set; }

        public Boolean IsTaxPayerLandlord { get; internal set; }

        public ReferenceDataStringValue BusinessName { get; internal set; }

        public ReferenceDataStringValue BusinessRegOffice { get; internal set; }

        public ReferenceDataStringValue CorporateHouseAddress { get; internal set; }

        public ReferenceDataStringValue CorporateStreetName { get; internal set; }

        public ReferenceDataStringValue CorporateCity { get; internal set; }

        public ReferenceDataStringValue CorporateLGA { get; internal set; }

        public ReferenceDataStringValue CorporateOrganizationType { get; internal set; }

        public ReferenceDataStringValue CorporateTin { get; internal set; }

        public ReferenceDataStringValue CorporateContactName { get; internal set; }

        public ReferenceDataStringValue CorporatePhoneNumber { get; internal set; }

        public ReferenceDataStringValue ContactEmail { get; internal set; }

        public ReferenceDataStringValue BusinessCommencement { get; internal set; }

        public ReferenceDataStringValue NumberEmployees { get; internal set; }

        public ReferenceDataStringValue DirectorTitle { get; internal set; }

        public ReferenceDataStringValue DirectorSex { get; internal set; }

        public ReferenceDataStringValue DirectorSurname { get; internal set; }

        public ReferenceDataStringValue DirectorFirstName { get; internal set; }

        public ReferenceDataStringValue DirectorMiddleName { get; internal set; }

        public ReferenceDataStringValue DirectorNationality { get; internal set; }

        public ReferenceDataStringValue DirectorPhone1 { get; internal set; }

        public ReferenceDataStringValue DirectorPhone2 { get; internal set; }

        public ReferenceDataStringValue DirectorEmail { get; internal set; }

        public ReferenceDataStringValue DirectorAddress { get; internal set; }

        public ReferenceDataStringValue DirectorCity { get; internal set; }

        public ReferenceDataStringValue CorporateIncomeYr1 { get; internal set; }

        public ReferenceDataStringValue CorporateIncomeYr2 { get; internal set; }

        public ReferenceDataStringValue CorporateIncomeYr3 { get; internal set; }

        public ReferenceDataStringValue SolProSouIncome { get; internal set; }

        public ReferenceDataStringValue AnnualGross { get; internal set; }

        public ReferenceDataStringValue TaxEntityCategory { get; internal set; }

        public List<TypeOfTaxPaidMappingLineRecordModel> TypeOfTaxPaidMappingList { get; internal set; }
    }
}
