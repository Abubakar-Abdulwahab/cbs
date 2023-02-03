using NHibernate.Criterion;
using Orchard;
using Orchard.Logging;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.TIN.Controllers.Handlers.Contracts;
using Parkway.CBS.TIN.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Web;

namespace Parkway.CBS.TIN.Controllers.Handlers
{
    public class TINHandler : ITINHandler
    {
        private readonly ITINFormManager<TINRegistrationForm> _tinFormManager;
        private readonly IOrchardServices _orchardServices;

        public ILogger Logger { get; set; }


        public TINHandler(IOrchardServices orchardServices, ITINFormManager<TINRegistrationForm> tinFormManager)
        {
            _tinFormManager = tinFormManager;
            Logger = NullLogger.Instance;
            _orchardServices = orchardServices;
        }


        private void RegisterTIN(TINFormViewModel formData)
        {
            var model = new TINFormViewModel();

            try
            {
                //build applicant model
                Logger.Information(string.Format("Building applicant model from TINFormViewModel"));

                var appliantFormData = PopulateApplicantFormData(formData);

                //create applicant
                _tinFormManager.SaveApplicantRecord(appliantFormData);
                Logger.Information(string.Format("Created {0} applicant with applicantId {1}", appliantFormData.FirstName.ToString(), appliantFormData.Id));

                #region address
                //build residential address model
                Logger.Information(string.Format("Building residential address model from TINViewModel"));
                var residentialAddresss = new Address
                {
                    State = formData.ResidentialAddress.State,
                    City = formData.ResidentialAddress.City,
                    StreetName = formData.ResidentialAddress.StreetName,
                    HouseNumber = formData.ResidentialAddress.HouseNumber,
                    Applicant = appliantFormData
                };

                //save residential address model
                _tinFormManager.SaveAddressRecord(residentialAddresss);
                Logger.Information(string.Format("Created residential address"));

                //build representative address model
                Logger.Information(string.Format("Building representativve address model from TINViewModel"));
                var representativeAddress = new Address
                {
                    State = formData.RepresentativeAddress.State,
                    City = formData.RepresentativeAddress.City,
                    StreetName = formData.RepresentativeAddress.StreetName,
                    HouseNumber = formData.RepresentativeAddress.HouseNumber,
                    Applicant = appliantFormData
                };

                //save residential address model
                _tinFormManager.SaveAddressRecord(representativeAddress);
                Logger.Information(string.Format("Created representative address"));

                //build business address model
                Logger.Information(string.Format("Building business address model from TINViewModel"));
                var BusinessAddress = new Address
                {
                    State = formData.BusinessAddress.State,
                    City = formData.BusinessAddress.City,
                    StreetName = formData.BusinessAddress.StreetName,
                    HouseNumber = formData.BusinessAddress.HouseNumber,
                    Applicant = appliantFormData
                };

                //save business address model
                _tinFormManager.SaveAddressRecord(BusinessAddress);
                Logger.Information(string.Format("Created business address"));

#endregion

                var dependantSplitTableRowsempty = new string[0];

                var dependantSplitTableRows = formData.DependantsTable != null ? formData.DependantsTable.Split('β') : dependantSplitTableRowsempty;
                var dependantSplitTableRowscount = dependantSplitTableRows.Length / 7;

                var a = 0;

                //build tin model
                Logger.Information(string.Format("Building tin registration form model from TINViewModel"));

                
                var tinRegistrationFormData = PopulateTINRegistrationForm(formData, appliantFormData, BusinessAddress, residentialAddresss, representativeAddress);

                    _tinFormManager.SaveTINRecord(tinRegistrationFormData);
                    Logger.Information(string.Format("Created and saved tin registration form to the db "));


                    var assetSplitTableRowsempty = new string[0];

                    var assetSplitTableRows = formData.AssetsTable != null ? formData.AssetsTable.Split('β') : assetSplitTableRowsempty;
                    var assetSplitTableRowscount = assetSplitTableRows.Length / 4;
                    int b = 0;

                    for (int i = 0; i < assetSplitTableRowscount; i++)
                    {
                        Asset AssetsRecord = new Asset();
                        AssetsRecord.TypeOfAsset = assetSplitTableRows[a];
                        a++;
                        AssetsRecord.LocationOfAsset = assetSplitTableRows[a];
                        a++;
                        AssetsRecord.MarketValue = string.IsNullOrWhiteSpace(assetSplitTableRows[a]) ? 0 : Convert.ToDecimal(assetSplitTableRows[a]);
                        a++;
                        AssetsRecord.OwnershipDate = string.IsNullOrWhiteSpace(assetSplitTableRows[a]) ? SqlDateTime.MinValue.Value : Convert.ToDateTime(assetSplitTableRows[a]);
                        a++;
                        AssetsRecord.TIN = tinRegistrationFormData;
                        _tinFormManager.SaveAssetsRecord(AssetsRecord);
                    }

                    var taxEntity = PopulateTaxEntityData(formData);

                    //get edpoint from IfeTayo
                    //add this tomorrow
                   var taxEntityResult = _tinFormManager.SaveTaxEntityRecord(taxEntity);
            }
            catch (Exception ex)
            {
                Logger.Error("An error {0} occured", ex.Message);
                Logger.Error(ex.StackTrace, ex);
                throw;
            }
        }

        /// <summary>
        /// Populate Tax Entity Record
        /// </summary>
        /// <param name="formData"></param>
        /// <returns></returns>
        private TaxEntity PopulateTaxEntityData(TINFormViewModel formData)
        {
            try
            {
                Logger.Debug("About to pupulate TaxEntity data");
                return new TaxEntity
                {
                    Address = ($"{formData.ResidentialAddress.HouseNumber}  {formData.ResidentialAddress.StreetName} {formData.ResidentialAddress.City}  {formData.ResidentialAddress.State}"),
                    Email = formData.Applicant.Email,
                    PhoneNumber = formData.Applicant.Phone,
                    Recipient = string.IsNullOrWhiteSpace(formData.Applicant.CompanyName) ? ($"{formData.Applicant.FirstName} {formData.Applicant.MiddleName} {formData.Applicant.LastName}") : formData.Applicant.CompanyName,
                    TaxEntityCategory = _tinFormManager.GetTaxCategory("Individual") //change this to accomodate company option when we finally get the company form
                };
            }
            catch (Exception ex)
            {
                Logger.Error("Could not populate TaxEntity data");
                Logger.Error(ex.StackTrace, ex);
                throw;
            }
        }

        /// <summary>
        /// Populate TIN Registration Form
        /// </summary>
        /// <param name="formData"></param>
        /// <param name="appliantFormData"></param>
        /// <returns></returns>
        private TINRegistrationForm PopulateTINRegistrationForm(TINFormViewModel formData, Applicant appliantFormData, Address BusinessAddress, Address residentialAddresss, Address representativeAddress)
        {
            try
            {
                var dependantSplitTableRowsempty = new string[0];

                var dependantSplitTableRows = formData.DependantsTable != null ? formData.DependantsTable.Split('β') : dependantSplitTableRowsempty;
                var dependantSplitTableRowscount = dependantSplitTableRows.Length / 7;

                var a = 0;

                Logger.Debug("About to poulate TIn Form Data");

                if (dependantSplitTableRows.Length > 0)
                {
                    return new TINRegistrationForm
                    {
                        Applicant = appliantFormData,
                        BusinessAddress = BusinessAddress,
                        ResidentialAddress = residentialAddresss,
                        RepresentativeAddress = representativeAddress,

                        IssuingAuthority = formData.IssuingAuthority,
                        IssuanceDate = formData.IssuanceDate == null ? SqlDateTime.MinValue.Value : Convert.ToDateTime(formData.IssuanceDate),
                        ExpiryDate = formData.ExpiryDate == null ? SqlDateTime.MinValue.Value : Convert.ToDateTime(formData.ExpiryDate),
                        PlaceOfIssuance = formData.PlaceOfIssuance,
                        IssuingAuthorityIdentification = formData.IssuingAuthorityIdentification,
                        LastAssessmentDate = formData.LastAssessmentDate == null ? SqlDateTime.MinValue.Value : Convert.ToDateTime(formData.LastAssessmentDate),
                        LastAssessmentAmount = formData.LastAssessmentAmount,
                        LastPaymentDate = formData.LastPaymentDate == null ? SqlDateTime.MinValue.Value : Convert.ToDateTime(formData.LastPaymentDate),
                        //LastPaymentAmount = formData.LastPaymentAmount,
                        TaxType = formData.TaxType,
                        TaxRepresentativeName = formData.TaxRepresentativeName,
                        TaxRepresentativeTin = formData.TaxRepresentativeTin,
                        RepType = formData.RepType,
                        ReasonForRepresentation = formData.ReasonForRepresentation,
                        RepresentativePhoneNumber1 = formData.RepresentativePhoneNumber1,
                        RepresentativePhoneNumber2 = formData.RepresentativePhoneNumber2,
                        RepresentativeEmail = formData.RepresentativeEmail,
                        SourceOfIncome = formData.SourceOfIncome,
                        EmploymentDate = formData.EmploymentDate == null ? SqlDateTime.MinValue.Value : Convert.ToDateTime(formData.EmploymentDate),
                        EmployerName = formData.EmployerName,
                        EmployerTIN = formData.EmployerTIN,
                        BusinessName = formData.BusinessName,
                        BusinessCommencementDate = formData.BusinessCommencementDate == null ? SqlDateTime.MinValue.Value : Convert.ToDateTime(formData.BusinessCommencementDate),
                        BusinessSector = formData.BusinessSector,
                        LineOfBusiness = formData.LineOfBusiness,
                        NumberOfEmployees = formData.NumberOfEmployees,
                        OwnershipName = formData.OwnershipName,

                        CompanyTIN = formData.CompanyTIN,
                        SharePercentage = formData.SharePercentage,
                        OwnershipStartDate = formData.OwnershipStartDate == null ? SqlDateTime.MinValue.Value : Convert.ToDateTime(formData.OwnershipStartDate),
                        SpouseSurname = formData.SpouseSurname,
                        SpouseFirstName = formData.SpouseFirstName,
                        SpouseMiddleName = formData.SpouseMiddleName,
                        StartDate = formData.StartDate == null ? SqlDateTime.MinValue.Value : Convert.ToDateTime(formData.StartDate),
                        SpouseTIN = formData.SpouseTIN,

                        DepChildLastName = dependantSplitTableRows[0],
                        DepChild1FirstName = dependantSplitTableRows[1],
                        DepChildMiddleName = dependantSplitTableRows[2],
                        DepChildState = dependantSplitTableRows[3],
                        DepChildDateofBirth = string.IsNullOrWhiteSpace(dependantSplitTableRows[4]) ? SqlDateTime.MinValue.Value : Convert.ToDateTime(dependantSplitTableRows[4]),
                        DepChildTIN = dependantSplitTableRows[5],
                        DepChildRelationship = dependantSplitTableRows[6],

                        RegistrationDate = DateTime.Now,
                        //DepChildLastName1 = dependantSplitTableRows[7],
                        //DepChild1FirstName1 = dependantSplitTableRows[8],
                        //DepChildMiddleName1 = dependantSplitTableRows[9],
                        //DepChildState1 = dependantSplitTableRows[10],
                        DepChildDateofBirth1 = SqlDateTime.MinValue.Value,
                        //DepChildTIN1 = dependantSplitTableRows[12],
                        //DepChildRelationship1 = dependantSplitTableRows[13],

                        //DepChildLastName2 = dependantSplitTableRows[14],
                        //DepChild1FirstName2 = dependantSplitTableRows[15],
                        //DepChildMiddleName2 = dependantSplitTableRows[16],
                        //DepChildState2 = dependantSplitTableRows[17],
                        DepChildDateofBirth2 = SqlDateTime.MinValue.Value,
                        //DepChildTIN2 = dependantSplitTableRows[19],
                        //DepChildRelationship2 = dependantSplitTableRows[20],

                    };
                }

                return new TINRegistrationForm
                {
                    Applicant = appliantFormData,
                    BusinessAddress = BusinessAddress,
                    ResidentialAddress = residentialAddresss,
                    RepresentativeAddress = representativeAddress,

                    IssuingAuthority = formData.IssuingAuthority,
                    IssuanceDate = formData.IssuanceDate == null ? SqlDateTime.MinValue.Value : Convert.ToDateTime(formData.IssuanceDate),
                    ExpiryDate = formData.ExpiryDate == null ? SqlDateTime.MinValue.Value : Convert.ToDateTime(formData.ExpiryDate),
                    PlaceOfIssuance = formData.PlaceOfIssuance,
                    IssuingAuthorityIdentification = formData.IssuingAuthorityIdentification,
                    LastAssessmentDate = formData.LastAssessmentDate == null ? SqlDateTime.MinValue.Value : Convert.ToDateTime(formData.LastAssessmentDate),
                    LastAssessmentAmount = formData.LastAssessmentAmount,
                    LastPaymentDate = formData.LastPaymentDate == null ? SqlDateTime.MinValue.Value : Convert.ToDateTime(formData.LastPaymentDate),
                    //LastPaymentAmount = formData.LastPaymentAmount,
                    TaxType = formData.TaxType,
                    TaxRepresentativeName = formData.TaxRepresentativeName,
                    TaxRepresentativeTin = formData.TaxRepresentativeTin,
                    RepType = formData.RepType,
                    ReasonForRepresentation = formData.ReasonForRepresentation,
                    RepresentativePhoneNumber1 = formData.RepresentativePhoneNumber1,
                    RepresentativePhoneNumber2 = formData.RepresentativePhoneNumber2,
                    RepresentativeEmail = formData.RepresentativeEmail,
                    SourceOfIncome = formData.SourceOfIncome,
                    EmploymentDate = formData.EmploymentDate == null ? SqlDateTime.MinValue.Value : Convert.ToDateTime(formData.EmploymentDate),
                    EmployerName = formData.EmployerName,
                    EmployerTIN = formData.EmployerTIN,
                    BusinessName = formData.BusinessName,
                    BusinessCommencementDate = formData.BusinessCommencementDate == null ? SqlDateTime.MinValue.Value : Convert.ToDateTime(formData.BusinessCommencementDate),
                    BusinessSector = formData.BusinessSector,
                    LineOfBusiness = formData.LineOfBusiness,
                    NumberOfEmployees = formData.NumberOfEmployees,
                    OwnershipName = formData.OwnershipName,

                    CompanyTIN = formData.CompanyTIN,
                    SharePercentage = formData.SharePercentage,
                    OwnershipStartDate = formData.OwnershipStartDate == null ? SqlDateTime.MinValue.Value : Convert.ToDateTime(formData.OwnershipStartDate),
                    SpouseSurname = formData.SpouseSurname,
                    SpouseFirstName = formData.SpouseFirstName,
                    SpouseMiddleName = formData.SpouseMiddleName,
                    StartDate = formData.StartDate == null ? SqlDateTime.MinValue.Value : Convert.ToDateTime(formData.StartDate),
                    SpouseTIN = formData.SpouseTIN,

                    DepChildLastName = "",
                    DepChild1FirstName = "",
                    DepChildMiddleName = "",
                    DepChildState = "",
                    DepChildDateofBirth =  SqlDateTime.MinValue.Value ,
                    DepChildTIN = "",
                    DepChildRelationship = "",

                    RegistrationDate = DateTime.Now,
                    //DepChildLastName1 = dependantSplitTableRows[7],
                    //DepChild1FirstName1 = dependantSplitTableRows[8],
                    //DepChildMiddleName1 = dependantSplitTableRows[9],
                    //DepChildState1 = dependantSplitTableRows[10],
                    DepChildDateofBirth1 = SqlDateTime.MinValue.Value,
                    //DepChildTIN1 = dependantSplitTableRows[12],
                    //DepChildRelationship1 = dependantSplitTableRows[13],

                    //DepChildLastName2 = dependantSplitTableRows[14],
                    //DepChild1FirstName2 = dependantSplitTableRows[15],
                    //DepChildMiddleName2 = dependantSplitTableRows[16],
                    //DepChildState2 = dependantSplitTableRows[17],
                    DepChildDateofBirth2 = SqlDateTime.MinValue.Value,
                    //DepChildTIN2 = dependantSplitTableRows[19],
                    //DepChildRelationship2 = dependantSplitTableRows[20],

                };

            }
            catch (Exception ex)
            {
                Logger.Error("Could not populate TIN registration form");
                Logger.Error(ex.StackTrace, ex);
                throw;
            }
        }

        /// <summary>
        /// Populate Applicant FormData
        /// </summary>
        /// <param name="formData"></param>
        /// <returns></returns>
        private Applicant PopulateApplicantFormData(TINFormViewModel formData)
        {
            try
            {
                Logger.Debug("About to populate Applicant Form Data");
                return new Applicant
                {
                    //ApplicantType = formData.ApplicantType,
                    //is this really correct
                    //TaxEntityCategory = _taxPayerCategoryManager.Get(formData.Applicant.TaxEntityCategoryId),
                    LastName = formData.Applicant.LastName ?? "",
                    FirstName = formData.Applicant.FirstName ?? "",
                    MiddleName = formData.Applicant.MiddleName ?? "",

                    Nationality = formData.Applicant.Nationality ?? "",
                    Phone = formData.Applicant.Phone ?? "",
                    Phone2 = formData.Applicant.Phone2 ?? "",
                    StateOfOrigin = formData.Applicant.StateOfOrigin ?? "",
                    MaritalStatus = formData.Applicant.MaritalStatus,
                    DateOfBirth = (formData.Applicant.DateOfBirth == null) ? SqlDateTime.MinValue.Value : formData.Applicant.DateOfBirth,
                    Occupation = formData.Applicant.Occupation ?? "",
                    Sex = formData.Applicant.Sex,
                    Email = formData.Applicant.Email,
                    MotherMaidenName = formData.Applicant.MotherMaidenName,
                    MotherName = formData.Applicant.MotherName,

                    IdentificationType = formData.Applicant.IdentificationType,
                    IdentificationNumber = formData.Applicant.IdentificationNumber,

                    DateOfRegistration = DateTime.Now,
                    //CreatedAtUtc,
                    //UpdatedAtUtc
                };
            }
            catch (Exception ex)
            {
                Logger.Error("An error occurred, could not populate model");
                Logger.Error(ex.StackTrace, ex);
                throw;
            }
           
        }

        /// <summary>
        /// do appropraite validation necessary, afterwards register the person for tin
        /// </summary>
        /// <param name="formData"></param>
        public bool TryRegisterTIN(TINFormViewModel formData)
        {
            //do appropraite validation necessary
            //afterwards register the person for tin
            try
            {
                RegisterTIN(formData);
                Logger.Debug("Successfully registered TIN Applicant");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error("Could not successfully register TIN Applicant");
                Logger.Error(ex.Message, ex);
                return false;
            }
            
        }

        /// <summary>
        /// Gets the ID of the last generated tin
        /// </summary>
        /// <returns></returns>
        public long GetLastGeneratedTin()
        {
            try
            {
                return _tinFormManager.GetCollection(x => x.Id > 0).ToList().OrderByDescending(x => x.Id).FirstOrDefault().Id;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.StackTrace, ex);
                throw;
            }
        }

        /// <summary>
        /// Get all TIN Applicants record
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TINApplicantReportModel> GetTINApplicantReport()
        {
            var tinViewRecords = new List<TINApplicantReportModel>();
            try
            {
                var taxPayerRecords = _tinFormManager.GetAllTINApplicants().ToList();

                foreach (var taxRecord in taxPayerRecords)
                {
                    var tin = new TINApplicantReportModel
                    {
                        TINId = taxRecord.Id,
                        //ApplicantName = string.IsNullOrWhiteSpace(taxRecord.CompanyName) ? taxRecord.FirstName + " " + taxRecord.LastName : taxRecord.CompanyName,
                        FirstName = taxRecord.Applicant.FirstName,
                        LastName = taxRecord.Applicant.LastName,
                        Email = taxRecord.Applicant.Email,
                        DateOfRegistration = Convert.ToDateTime(taxRecord.Applicant.DateOfRegistration),
                        Phone = taxRecord.Applicant.Phone,
                        TIN = taxRecord.TIN
                        //ApplicantType = taxRecord.ApplicantType
                    };
                    tinViewRecords.Add(tin);
                }

                return tinViewRecords;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.StackTrace, ex);
                return tinViewRecords;
            }
        }

        /// <summary>
        /// do a serach on the TIN Registration Form Table
        /// </summary>
        /// <param name="searchData"></param>
        /// <returns></returns>
        public IEnumerable<TINApplicantReportModel> TINApplicantReportSearch(TINSearchParameters searchData)
        {
            var tinReport = new List<TINApplicantReportModel>();
            try
            {
                var firstName = searchData.FirstName ?? "";
                var lastName = searchData.LastName ?? "";
                var phone = searchData.PhoneNumber ?? "";
                var startDate = Convert.ToDateTime(searchData.StartDate).Date;
                var endDate = Convert.ToDateTime(searchData.EndDate).Date;

                // query to perform a join and return all data 

                //perform search operation on returned data

                var result = _tinFormManager.TINApplicantSearch(firstName, lastName, phone, searchData.StartDate, searchData.EndDate);

                if (result != null)
                {
                    return result;
                }

                return tinReport;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.StackTrace, ex);
                return tinReport; 
            }
        }

        /// <summary>
        /// Uodate TIN Table and TaxPayerEntity table with the applicant TIN
        /// </summary>
        /// <param name="tINId"></param>
        /// <param name="TINValue"></param>
        public bool UpdateTIN(long tINId, string TINValue)
        {
            try
            {
                //include logic
                var result = _tinFormManager.UpdateTINApplicantRecord(tINId, TINValue);
                if (result == true)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.StackTrace, ex);
                return false;
            }
        }
    }
}