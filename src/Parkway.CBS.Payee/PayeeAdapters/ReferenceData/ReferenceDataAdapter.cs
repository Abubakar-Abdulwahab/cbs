using ExcelDataReader;
using Newtonsoft.Json;
using Parkway.CBS.Payee.PayeeAdapters.Contracts;
using Parkway.CBS.Payee.PayeeAdapters.ReferenceData;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Payee.ReferenceDataImplementation
{
    public class ReferenceDataAdapter : IReferenceDataAdapter
    {
        private const int templateHeaderCount = 74;
        protected readonly string[] templateHeaderNames =
                {
            "Row Id".ToLower() , "Completed".ToLower(), "Completed By".ToLower(), "Started".ToLower(),
            "Received".ToLower(), "Completed At".ToLower(), "Title".ToLower(), "Sex".ToLower(), "Surname".ToLower(),
            "FirstName".ToLower(), "MiddleName".ToLower(), "Nationality".ToLower(), "PhoneNumber1".ToLower(), "PhoneNumber2".ToLower(),
            "Email".ToLower(), "Tin".ToLower(), "HouseNo".ToLower(), "StreetName".ToLower(), "City".ToLower(), "LGA".ToLower(), "EmploymentStatus".ToLower(), "EmployerName".ToLower(), "EmployerAddress".ToLower(), "PropertyType".ToLower(), "PropertyStructure".ToLower(), "PropertyStructureNumber".ToLower(), "PropertyAddress".ToLower(), "PropertyRent".ToLower(), "PeriodCoveredFrom".ToLower(), "PeriodCoveredTo".ToLower(), "OwnerTitle".ToLower(), "OwnerSex".ToLower(), "OwnerSurname".ToLower(), "OwnerFirstName".ToLower(),
            "OwnerMiddleName".ToLower(), "OwnerNationality".ToLower(), "OwnerPhone1".ToLower(), "OwnerPhone2".ToLower(), "OwnerEmail".ToLower(), "OwnerEmploymentStatus".ToLower(), "OwnerTin".ToLower(), "OwnerEmployerName".ToLower(), "OwnerEmployerAddress".ToLower(),
            "TypeOfTaxPaid".ToLower(), "EvidenceProvided".ToLower(), "BusinessName".ToLower(), "BusinessRegOffice".ToLower(), "CorporateHouseAddress".ToLower(), "CorporateStreetName".ToLower(), "CorporateCity".ToLower(), "CorporateLGA".ToLower(), "CorporateOrganizationType".ToLower(), "CorporateTin".ToLower(), "CorporateContactName".ToLower(), "CorporatePhoneNumber".ToLower(), "ContactEmail".ToLower(), "BusinessCommencement".ToLower(), "NumberEmployees".ToLower(),
            "DirectorTitle".ToLower(), "DirectorSex".ToLower(), "DirectorSurname".ToLower(), "DirectorFirstName".ToLower(), "DirectorMiddleName".ToLower(), "DirectorNationality".ToLower(), "DirectorPhone1".ToLower(), "DirectorPhone2".ToLower(), "DirectorEmail".ToLower(), "DirectorAddress".ToLower(), "DirectorCity".ToLower(), "CorporateIncomeYr1".ToLower(), "CorporateIncomeYr2".ToLower(), "CorporateIncomeYr3".ToLower(), "SolProSouIncome".ToLower(), "AnnualGross".ToLower()
        };

        /// <summary>
        /// Get the response to reading the file and processing the file from the file path
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>ReferenceDataResponse</returns>
        public ReferenceDataResponse GetReferenceDataResponseModels(string filePath)
        {
            return GetReferenceDataRecords(filePath);
        }

        /// <summary>
        /// Validate the headers and read the file.
        /// <para>
        /// this method validates the headers of the file, if headers are ok, 
        /// it proceeds to read each row on the excel file
        /// </para>
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        protected ReferenceDataResponse GetReferenceDataRecords(string filePath)
        {
            DataSet result = new DataSet();
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateCsvReader(stream))
                { result = reader.AsDataSet(); }
            }

            var sheet1 = result.Tables[0];
            var rows = sheet1.Rows;

            Dictionary<string, HeaderValidationModel> headers = new Dictionary<string, HeaderValidationModel>();
            for (int i = 0; i < templateHeaderCount; i++)
            {
                headers.Add(templateHeaderNames[i], new HeaderValidationModel { });
            }


            ValidateTemplateHeaders(rows[0], ref headers);

            var invalidsHeaders = headers.Where(k => k.Value.HeaderPresent == false);
            if (invalidsHeaders.Count() > 0)
            {
                var msg = invalidsHeaders.Select(x => x.Key + " header not found").ToArray();
                return new ReferenceDataResponse { HeaderValidateObject = new HeaderValidateObject { Error = true, ErrorMessage = string.Join("\n", msg) } };
            }

            rows.RemoveAt(0);
            var col = sheet1.Columns;
            ConcurrentStack<ReferenceDataLineRecordModel> referenceDataRecords = new ConcurrentStack<ReferenceDataLineRecordModel> { };
            IEnumerable<DataRow> irows = rows.OfType<DataRow>();
            Parallel.ForEach(irows, (item) =>
            {
                List<string> lineValues = new List<string>();
                for (int i = 0; i < templateHeaderCount; i++)
                {
                    lineValues.Add(item.ItemArray[headers[templateHeaderNames[i]].IndexOnFile].ToString());
                }

                var referenceDataLineRecord = GetReferenceDataRecords(lineValues);
                referenceDataRecords.Push(referenceDataLineRecord);

                if (referenceDataLineRecord.IsTaxPayerLandlord == false)
                {
                    ReferenceDataLineRecordModel referenceDataLineRecordCopy = new ReferenceDataLineRecordModel();
                    ObjectCopier<ReferenceDataLineRecordModel, ReferenceDataLineRecordModel>.Copy(referenceDataLineRecord, referenceDataLineRecordCopy);
                    referenceDataLineRecordCopy.Surname = referenceDataLineRecord.OwnerSurname;
                    referenceDataLineRecordCopy.Firstname = referenceDataLineRecord.OwnerFirstname;
                    referenceDataLineRecordCopy.Middlename = referenceDataLineRecord.OwnerMiddlename;
                    referenceDataLineRecordCopy.PhoneNumber1 = referenceDataLineRecord.OwnerPhoneNumber1;
                    referenceDataLineRecordCopy.PhoneNumber2 = referenceDataLineRecord.OwnerPhoneNumber2;
                    referenceDataLineRecordCopy.EmailAddress = referenceDataLineRecord.OwnerEmailAddress;
                    referenceDataLineRecordCopy.TIN = referenceDataLineRecord.OwnerTIN;
                    referenceDataLineRecordCopy.IsTaxPayerLandlord = true;

                    referenceDataRecords.Push(referenceDataLineRecordCopy);
                }

            });

            return new ReferenceDataResponse { ReferenceDataLineRecords = referenceDataRecords, HeaderValidateObject = new HeaderValidateObject { } };
        }

        /// <summary>
        /// validate the file for the header information
        /// <para>we loop through the pre-defined headers given, then we set the index of the header to where on the file the header apppears</para>
        /// </summary>
        /// <param name="header">header object from file</param>
        /// <param name="headers">pre-defined headers that we are expecting</param>
        protected void ValidateTemplateHeaders(DataRow header, ref Dictionary<string, HeaderValidationModel> headers)
        {
            string errorMessage = string.Empty;
            int counter = -1;
            foreach (object item in header.ItemArray)
            {
                if (item is DBNull) { break; }
                counter++;
                string sItem = ((string)item).Trim().ToLower();

                for (int i = 0; i < templateHeaderCount; i++)
                {
                    if (templateHeaderNames[i].Equals(sItem)) { headers[templateHeaderNames[i]] = new HeaderValidationModel { HeaderPresent = true, IndexOnFile = counter }; continue; }
                }
            }
        }

        /// <summary>
        /// Read the value on each row of the excel
        /// </summary>
        /// <param name="lineValues"></param>
        /// <returns>ReferenceDataLineRecordModel</returns>
        protected ReferenceDataLineRecordModel GetReferenceDataRecords(List<string> lineValues)
        {
            int maximumZeroPadding = 1;
            var referenceData = new ReferenceDataLineRecordModel();

            var rowId = GetStringValue(lineValues.ElementAt(0), "Row Id");
            referenceData.Row_Id = rowId;

            var completed = GetStringValue(lineValues.ElementAt(1), "Completed");
            referenceData.Completed = completed;

            var completedBy = GetStringValue(lineValues.ElementAt(2), "Completed By");
            referenceData.CompletedBy = completedBy;

            var started = GetStringValue(lineValues.ElementAt(3), "Started");
            referenceData.Started = started;

            var received = GetStringValue(lineValues.ElementAt(4), "Received");
            referenceData.Received = received;

            var completedAt = GetStringValue(lineValues.ElementAt(5), "Completed At");
            referenceData.CompletedAt = completedAt;

            var title = GetStringValue(lineValues.ElementAt(6), "Title");
            referenceData.Title = title;

            var sex = GetStringValue(lineValues.ElementAt(7), "Sex");
            referenceData.Sex = sex;

            var surname = GetStringValue(lineValues.ElementAt(8), "Surname");
            referenceData.Surname = surname;

            var firstName = GetStringValue(lineValues.ElementAt(9), "FirstName");
            referenceData.Firstname = firstName;

            var middleName = GetStringValue(lineValues.ElementAt(10), "MiddleName");
            referenceData.Middlename = middleName;

            var nationality = GetStringValue(lineValues.ElementAt(11), "Nationality");
            referenceData.Nationality = nationality;

            //Append the leading zero to the phone number
            var phoneNumber1 = GetStringValue(lineValues.ElementAt(12), "PhoneNumber1");
            referenceData.PhoneNumber1 = ZeroPadUp(phoneNumber1.Value, maximumZeroPadding);

            var phoneNumber2 = GetStringValue(lineValues.ElementAt(13), "PhoneNumber2");
            referenceData.PhoneNumber2 = ZeroPadUp(phoneNumber2.Value, maximumZeroPadding);

            var email = GetStringValue(lineValues.ElementAt(14), "Email");
            referenceData.EmailAddress = email;

            var tin = GetStringValue(lineValues.ElementAt(15), "Tin");
            referenceData.TIN = tin;

            var houseNo = GetStringValue(lineValues.ElementAt(16), "HouseNo");
            referenceData.HouseNo = houseNo;

            var streetName = GetStringValue(lineValues.ElementAt(17), "StreetName");
            referenceData.StreetName = streetName;

            var city = GetStringValue(lineValues.ElementAt(18), "City");
            referenceData.City = city;

            var lga = GetStringValue(lineValues.ElementAt(19), "LGA");
            referenceData.LGA = lga;

            var employmentStatus = GetStringValue(lineValues.ElementAt(20), "EmploymentStatus");
            referenceData.EmploymentStatus = employmentStatus;

            var employerName = GetStringValue(lineValues.ElementAt(21), "EmployerName");
            referenceData.EmployerName = employerName;

            var employerAddress = GetStringValue(lineValues.ElementAt(22), "EmployerAddress");
            referenceData.EmployerAddress = employerAddress;

            var propertyType = GetStringValue(lineValues.ElementAt(23), "PropertyType");
            referenceData.PropertyType = propertyType;

            var propertyStructure = GetStringValue(lineValues.ElementAt(24), "PropertyStructure");
            referenceData.PropertyStructure = propertyStructure;

            var propertyStructureNumber = GetStringValue(lineValues.ElementAt(25), "PropertyStructureNumber");
            referenceData.PropertyStructureNumber = propertyStructureNumber;

            var propertyAddress = GetStringValue(lineValues.ElementAt(26), "PropertyAddress");
            referenceData.PropertyAddress = propertyAddress;

            decimal rentAmount = 0;
            var PropertyRent = GetStringValue(lineValues.ElementAt(27), "PropertyRent");
            decimal.TryParse(PropertyRent.Value, out rentAmount);
            referenceData.PropertyRentAmount = rentAmount;

            var rentStartDate = GetStringValue(lineValues.ElementAt(28), "PeriodCoveredFrom");
            referenceData.RentStartDate = rentStartDate;

            var rentEndDate = GetStringValue(lineValues.ElementAt(29), "PeriodCoveredTo");
            referenceData.RentEndDate = rentEndDate;

            var ownerTitle = GetStringValue(lineValues.ElementAt(30), "OwnerTitle");
            referenceData.OwnerTitle = ownerTitle;

            var ownerSex = GetStringValue(lineValues.ElementAt(31), "OwnerSex");
            referenceData.OwnerSex = ownerSex;

            var OwnerSurname = GetStringValue(lineValues.ElementAt(32), "OwnerSurname");
            referenceData.OwnerSurname = OwnerSurname;

            var ownerFirstName = GetStringValue(lineValues.ElementAt(33), "OwnerFirstName");
            referenceData.OwnerFirstname = ownerFirstName;

            var ownerMiddleName = GetStringValue(lineValues.ElementAt(34), "OwnerMiddleName");
            referenceData.OwnerMiddlename = ownerMiddleName;

            var ownerNationality = GetStringValue(lineValues.ElementAt(35), "OwnerNationality");
            referenceData.OwnerNationality = ownerNationality;

            var ownerPhone1 = GetStringValue(lineValues.ElementAt(36), "OwnerPhone1");
            referenceData.OwnerPhoneNumber1 = ZeroPadUp(ownerPhone1.Value, 1);

            var ownerPhone2 = GetStringValue(lineValues.ElementAt(37), "OwnerPhone2");
            referenceData.OwnerPhoneNumber2 = ZeroPadUp(ownerPhone2.Value, 1);

            var ownerEmail = GetStringValue(lineValues.ElementAt(38), "OwnerEmail");
            referenceData.OwnerEmailAddress = ownerEmail;

            var ownerEmploymentStatus = GetStringValue(lineValues.ElementAt(39), "OwnerEmploymentStatus");
            referenceData.OwnerEmploymentStatus = ownerEmploymentStatus;

            var ownerTin = GetStringValue(lineValues.ElementAt(40), "OwnerTin");
            referenceData.OwnerTIN = ownerTin;

            var ownerEmployerName = GetStringValue(lineValues.ElementAt(41), "OwnerEmployerName");
            referenceData.OwnerEmployerName = ownerEmployerName;

            var ownerEmployerAddress = GetStringValue(lineValues.ElementAt(42), "OwnerEmployerAddress");
            referenceData.OwnerEmployerAddress = ownerEmployerAddress;

            var typeOfTaxPaid = GetStringValue(lineValues.ElementAt(43), "TypeOfTaxPaid");
            referenceData.TypeOfTaxPaid = typeOfTaxPaid;

            var evidenceProvided = GetStringValue(lineValues.ElementAt(44), "EvidenceProvided");
            referenceData.EvidenceProvided = string.IsNullOrEmpty(evidenceProvided.Value) ? false : true;

            var IsTaxPayerEqualOwner = GetStringValue(lineValues.ElementAt(44), "EvidenceProvided");
            referenceData.IsTaxPayerLandlord = string.IsNullOrEmpty(OwnerSurname.Value + ownerFirstName.Value) ? true : false;

            var businessName = GetStringValue(lineValues.ElementAt(45), "BusinessName");
            referenceData.BusinessName = businessName;

            var businessRegOffice = GetStringValue(lineValues.ElementAt(46), "BusinessRegOffice");
            referenceData.BusinessRegOffice = businessRegOffice;

            var corporateHouseAddress = GetStringValue(lineValues.ElementAt(47), "CorporateHouseAddress");
            referenceData.CorporateHouseAddress = corporateHouseAddress;

            var corporateStreetName = GetStringValue(lineValues.ElementAt(48), "CorporateStreetName");
            referenceData.CorporateStreetName = corporateStreetName;

            var corporateCity = GetStringValue(lineValues.ElementAt(49), "CorporateCity");
            referenceData.CorporateCity = corporateCity;

            var corporateLGA = GetStringValue(lineValues.ElementAt(50), "CorporateLGA");
            referenceData.CorporateLGA = corporateLGA;

            var corporateOrganizationType = GetStringValue(lineValues.ElementAt(51), "CorporateOrganizationType");
            referenceData.CorporateOrganizationType = corporateOrganizationType;

            var corporateTin = GetStringValue(lineValues.ElementAt(52), "CorporateTin");
            referenceData.CorporateTin = corporateTin;

            var corporateContactName = GetStringValue(lineValues.ElementAt(53), "CorporateContactName");
            referenceData.CorporateContactName = corporateContactName;

            var corporatePhoneNumber = GetStringValue(lineValues.ElementAt(54), "CorporatePhoneNumber");
            referenceData.CorporatePhoneNumber = corporatePhoneNumber;

            var contactEmail = GetStringValue(lineValues.ElementAt(55), "ContactEmail");
            referenceData.ContactEmail = contactEmail;

            var businessCommencement = GetStringValue(lineValues.ElementAt(56), "BusinessCommencement");
            referenceData.BusinessCommencement = businessCommencement;

            var numberEmployees = GetStringValue(lineValues.ElementAt(57), "NumberEmployees");
            referenceData.NumberEmployees = numberEmployees;

            var directorTitle = GetStringValue(lineValues.ElementAt(58), "DirectorTitle");
            referenceData.DirectorTitle = directorTitle;

            var directorSex = GetStringValue(lineValues.ElementAt(59), "DirectorSex");
            referenceData.DirectorSex = directorSex;

            var directorSurname = GetStringValue(lineValues.ElementAt(60), "DirectorSurname");
            referenceData.DirectorSurname = directorSurname;

            var directorFirstName = GetStringValue(lineValues.ElementAt(61), "DirectorFirstName");
            referenceData.DirectorFirstName = directorFirstName;

            var directorMiddleName = GetStringValue(lineValues.ElementAt(62), "DirectorMiddleName");
            referenceData.DirectorMiddleName = directorMiddleName;

            var directorNationality = GetStringValue(lineValues.ElementAt(63), "DirectorNationality");
            referenceData.DirectorNationality = directorNationality;

            var directorPhone1 = GetStringValue(lineValues.ElementAt(64), "DirectorPhone1");
            referenceData.DirectorPhone1 = directorPhone1;

            var directorPhone2 = GetStringValue(lineValues.ElementAt(65), "DirectorPhone2");
            referenceData.DirectorPhone2 = directorPhone2;

            var directorEmail = GetStringValue(lineValues.ElementAt(66), "DirectorEmail");
            referenceData.DirectorEmail = directorEmail;

            var directorAddress = GetStringValue(lineValues.ElementAt(67), "DirectorAddress");
            referenceData.DirectorAddress = directorAddress;

            var directorCity = GetStringValue(lineValues.ElementAt(68), "DirectorCity");
            referenceData.DirectorCity = directorCity;

            var corporateIncomeYr1 = GetStringValue(lineValues.ElementAt(69), "CorporateIncomeYr1");
            referenceData.CorporateIncomeYr1 = corporateIncomeYr1;

            var corporateIncomeYr2 = GetStringValue(lineValues.ElementAt(70), "CorporateIncomeYr2");
            referenceData.CorporateIncomeYr2 = corporateIncomeYr2;

            var corporateIncomeYr3 = GetStringValue(lineValues.ElementAt(71), "CorporateIncomeYr3");
            referenceData.CorporateIncomeYr3 = corporateIncomeYr3;

            var solProSouIncome = GetStringValue(lineValues.ElementAt(72), "SolProSouIncome");
            referenceData.SolProSouIncome = solProSouIncome;

            var annualGross = GetStringValue(lineValues.ElementAt(73), "AnnualGross");
            referenceData.AnnualGross = annualGross;

            //Determine the Refrence Data category based on surname and firstname
            referenceData.TaxEntityCategory = string.IsNullOrEmpty(businessName.Value + businessRegOffice.Value) ? GetStringValue(ConfigurationManager.AppSettings["IndividualTaxCategoryId"], "TaxEntityCategory") : GetStringValue(ConfigurationManager.AppSettings["CorporateTaxCategoryId"], "TaxEntityCategory");
            if (!string.IsNullOrEmpty(typeOfTaxPaid.Value) && !string.IsNullOrEmpty(evidenceProvided.Value))
            {
                List<TypeOfTaxPaidMappingLineRecordModel> revenueHeadMappingList = new List<TypeOfTaxPaidMappingLineRecordModel>();
                var typeOfTaxPaidSplit = typeOfTaxPaid.Value.Split('|');

                foreach(var taxPaid in typeOfTaxPaidSplit)
                {
                    int taxPaidId = 0;
                    int.TryParse(taxPaid, out taxPaidId);
                    TypeOfTaxPaidMappingLineRecordModel taxPaidMapping = new TypeOfTaxPaidMappingLineRecordModel();
                    taxPaidMapping.ReferenceDataTypeOfTaxPaid = taxPaidId;
                    revenueHeadMappingList.Add(taxPaidMapping);
                }
                referenceData.TypeOfTaxPaidMappingList = revenueHeadMappingList;
            }

            return referenceData;
        }

        /// <summary>
        /// Get the trim string value
        /// </summary>
        /// <param name="stringValue"></param>
        /// <param name="headerValue"></param>
        /// <returns></returns>
        protected virtual ReferenceDataStringValue GetStringValue(string stringValue, string headerValue)
        {
            return new ReferenceDataStringValue { Value = string.IsNullOrEmpty(stringValue) ? stringValue : stringValue.Trim() };
        }

        /// <summary>
        /// Pad a sring value with max padding zeros.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="maxPadding"></param>
        /// <param name="prefix"></param>
        /// <returns>String</returns>
        public static ReferenceDataStringValue ZeroPadUp(string value, int maxPadding, string prefix = null)
        {
            string result = value.PadLeft(value.Length + maxPadding, '0');
            if (!string.IsNullOrEmpty(prefix)) { return new ReferenceDataStringValue { Value = string.IsNullOrEmpty(value) ? value : prefix + result.Trim() }; }
            return new ReferenceDataStringValue { Value = string.IsNullOrEmpty(value) ? value : result.Trim() };
        }

    }


    public class ReferenceDataStringValue
    {
        public string Value { get; internal set; }
    }

    public class ObjectCopier<TParent, TChild> where TParent : class where TChild : class
    {
        public static void Copy(TParent parent, TChild child)
        {
            var parentProperties = parent.GetType().GetProperties();
            var childProperties = child.GetType().GetProperties();

            foreach (var parentProperty in parentProperties)
            {
                foreach (var childProperty in childProperties)
                {
                    if (parentProperty.Name == childProperty.Name && parentProperty.PropertyType == childProperty.PropertyType)
                    {
                        childProperty.SetValue(child, parentProperty.GetValue(parent));
                        break;
                    }
                }
            }
        }
    }

}
