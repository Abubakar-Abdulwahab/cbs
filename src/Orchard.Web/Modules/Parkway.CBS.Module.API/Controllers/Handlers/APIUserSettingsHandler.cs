using Orchard.Logging;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core;
using Parkway.CBS.Core.Lang;
using Newtonsoft.Json;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Module.API.Controllers.Handlers.Contracts;
using Parkway.CBS.Core.DataFilters.TaxPayerReport;

namespace Parkway.CBS.Module.API.Controllers.Handlers
{
    public class APIUserSettingsHandler : BaseAPIHandler, IAPIUserSettingsHandler
    {
        private readonly ITaxEntityCategoryManager<TaxEntityCategory> _taxCategoryManager;
        private readonly ICoreUserService _coreUserService;
        private readonly ITaxPayerReportFilter _taxpayerReportFilter;


        public APIUserSettingsHandler(IAdminSettingManager<ExpertSystemSettings> settingsRepository, ITaxEntityCategoryManager<TaxEntityCategory> taxCategoryManager, ICoreUserService coreUserService, ITaxPayerReportFilter taxpayerReportFilter) : base(settingsRepository)
        {
            _taxCategoryManager = taxCategoryManager;
            _settingsRepository = settingsRepository;
            _coreUserService = coreUserService;
            _taxpayerReportFilter = taxpayerReportFilter;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// Create a CBS user
        /// </summary>
        /// <param name="callback">UserController</param>
        /// <param name="model">RegisterCBSUserModel</param>
        /// <param name="headerParams">dynamic</param>
        /// <returns>APIResponse</returns>
        public APIResponse CreateCBSUser(UserController callback, RegisterUserModel model, dynamic headerParams)
        {
            List<ErrorModel> errors = new List<ErrorModel>();
            System.Net.HttpStatusCode responseCode = System.Net.HttpStatusCode.BadRequest;
            ErrorCode errorCode = new ErrorCode();
            if(model == null) { return new APIResponse { Error = true, ErrorCode = ErrorCode.PPVE.ToString(), ResponseObject = "No form data found", StatusCode = System.Net.HttpStatusCode.BadRequest }; }

            Logger.Error("registering cbs user for model " + JsonConvert.SerializeObject(model));
            try
            {
                Logger.Error("Validating model");
                callback.Validate(model);
                CheckModelState<UserController>(callback, ref errors);
                ////get tenant settings
                ExpertSystemSettings expertSystem = GetExpertSystem(headerParams.CLIENTID);
                //do check for hash
                string value = model.Password + model.Name + model.UserName + model.TIN + model.PhoneNumber + model.SCategoryIdentifier + model.CategoryIdentifier + headerParams.CLIENTID;
                if (!CheckHash(value, headerParams.SIGNATURE, expertSystem.ClientSecret))
                {
                    Logger.Error("Signature hash does not match " + value);
                    return new APIResponse { ErrorCode = ErrorCode.PPS1.ToString(), StatusCode = System.Net.HttpStatusCode.Forbidden, Error = true, ResponseObject = new List<ErrorModel> { { new ErrorModel { ErrorMessage = ErrorLang.couldnotcomputehash().ToString(), FieldName = "Signature" } } } };
                }

                TaxEntityCategory category = _taxCategoryManager.Get(cato => cato.StringIdentifier == model.SCategoryIdentifier);
                if (category == null) { throw new NoCategoryFoundException(); }

                var result = _coreUserService.TryCreateCBSUser(
                    new RegisterCBSUserModel
                    {
                        Address = model.Address,
                        CategoryIdentifier = model.CategoryIdentifier,
                        Password = model.Password,
                        Email = model.Email,
                        Name = model.Name,
                        PhoneNumber = model.PhoneNumber,
                        SCategoryIdentifier = model.SCategoryIdentifier,
                        TIN = model.TIN,
                        UserName = model.UserName
                    }, category, ref errors, expertSystem, model.RequestReference);

                if (errors.Count > 0) { throw new DirtyFormDataException(); }
                return new APIResponse { StatusCode = System.Net.HttpStatusCode.OK, ResponseObject = result };
            }
            #region catch clauses
            catch(TenantNotFoundException)
            {
                errorCode = ErrorCode.PPS1;
                errors.Add(new ErrorModel { FieldName = "ExpertSystem", ErrorMessage = "Could not find your client details" });
            }
            catch (NoCategoryFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                errorCode = ErrorCode.PPCAT404;
                errors.Add(new ErrorModel { FieldName = "SCategoryIdentifier", ErrorMessage = ErrorLang.categorynotfound().ToString() });
            }
            catch (CBSUserNotFoundException)
            {
                Logger.Error("CBS user not found");
                errorCode = ErrorCode.PPUSER404;
                errors.Add(new ErrorModel { FieldName = "", ErrorMessage = ErrorLang.usernotfound().ToString() });
            }
            catch (CBSUserAlreadyExistsException)
            {
                errorCode = ErrorCode.PPUSERALREADYEXISTS;
                errors.Add(new ErrorModel { ErrorMessage = ErrorLang.profilealreadyexists().ToString(), FieldName = "TIN" });
            }
            catch (PhoneNumberHasBeenTakenException)
            {
                errorCode = ErrorCode.PPUSERPHONEALREADYEXISTS;
                errors.Add(new ErrorModel { ErrorMessage = ErrorLang.phonenumberalreadyexists().ToString(), FieldName = "PhoneNumber" });
            }
            catch (AuthorizedUserNotFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                responseCode = System.Net.HttpStatusCode.NotFound;
                errorCode = ErrorCode.PPUSER404;
                errors.Add(new ErrorModel { FieldName = "LastUpdatedBy", ErrorMessage = ErrorLang.usernotfound().ToString() });
            }
            catch (DirtyFormDataException exception)
            {
                Logger.Error(exception, exception.Message);
                errorCode = ErrorCode.PPVE;
            }
            catch (CannotSaveTaxEntityException)
            {
                errorCode = ErrorCode.PPTAXENTITY500;
                errors.Add(new ErrorModel { FieldName = "User", ErrorMessage = ErrorLang.couldnotsavetaxentityrecord().ToString() });
            }
            catch (CouldNotSaveCBSUserException)
            {
                errorCode = ErrorCode.PPUSERCBS500;
                errors.Add(new ErrorModel { ErrorMessage = ErrorLang.couldnotcbsuser().ToString(), FieldName = "User" });
            }
            catch (AlreadyHasBillingException exception)
            {
                Logger.Error(exception, exception.Message);
                errorCode = ErrorCode.PPB2;
                errors.Add(new ErrorModel { FieldName = "RevenueHead", ErrorMessage = ErrorLang.alreadyhasbillinginfo().ToString() });
            }
            catch (CannotCreateStartSetupProcessBecauseRevenueHeadHasSubRevenueHeadsException exception)
            {
                Logger.Error(exception, exception.Message);
                errorCode = ErrorCode.PPR2;
                errors.Add(new ErrorModel { FieldName = "RevenueHead", ErrorMessage = ErrorLang.revenueheadhassubrevenueheads().ToString() });
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                errorCode = ErrorCode.PPIE;
                errors.Add(new ErrorModel { FieldName = "RevenueHead", ErrorMessage = ErrorLang.genericexception().ToString() });
            }
            #endregion
            return new APIResponse { ErrorCode = errorCode.ToString(), Error = true, StatusCode = responseCode, ResponseObject = errors };
        }


        /// <summary>
        /// Do model check
        /// </summary>
        /// <param name="userController"></param>
        /// <returns> List{ErrorModel}</returns>
        public List<ErrorModel> DoModelCheck(UserController callback)
        {
            return CheckModelStateWithoutException(callback);
        }


        /// <summary>
        /// Search for tax payer profile
        /// </summary>
        /// <param name="searchFilter">TaxPayerDetails</param>
        /// <param name="headerParams">dynamic</param>
        /// <returns>APIResponse</returns>
        public APIResponse SearchTaxProfilesByFilter(TaxProfilesSearchParams searchFilter, dynamic headerParams)
        {
            List<ErrorModel> errors = new List<ErrorModel>();
            System.Net.HttpStatusCode responseCode = System.Net.HttpStatusCode.BadRequest;
            ErrorCode errorCode = new ErrorCode();
            if (searchFilter == null) { return new APIResponse { Error = true, ErrorCode = ErrorCode.PPVE.ToString(), ResponseObject = new List<ErrorModel> { { new ErrorModel { ErrorMessage = ErrorLang.modelisempty().ToString(), FieldName = "SearchModel" } } }, StatusCode = System.Net.HttpStatusCode.BadRequest }; }

            string jsonStr = JsonConvert.SerializeObject(searchFilter);
            Logger.Error("searching for tax entity " + jsonStr);
            try
            {
                Logger.Error("Validating model");
                ////get tenant settings
                string clientSecret = _settingsRepository.GetClientSecretByClientId(headerParams.CLIENTID);
                if (string.IsNullOrEmpty(clientSecret)) { throw new TenantNotFoundException(); }

                //do check for hash
                if (!CheckHash(jsonStr, headerParams.SIGNATURE, clientSecret))
                {
                    Logger.Error("Signature hash does not match ");
                    return new APIResponse { ErrorCode = ErrorCode.PPS1.ToString(), StatusCode = System.Net.HttpStatusCode.Forbidden, Error = true, ResponseObject = new List<ErrorModel> { { new ErrorModel { ErrorMessage = ErrorLang.couldnotcomputehash().ToString(), FieldName = "Signature" } } } };
                }

                searchFilter.PageSize = searchFilter.PageSize <= 0 || searchFilter.PageSize > 100 ? 100 : searchFilter.PageSize;
                int skip = 0;
                searchFilter.Page = searchFilter.Page <= 0 ? 1 : searchFilter.Page;
                skip = (searchFilter.Page - 1) * searchFilter.PageSize;

                var searchRecords = _taxpayerReportFilter.GetReportForTaxProfiles(searchFilter, skip, searchFilter.PageSize);
                List<TaxEntityReportDetail> taxPayerReport = new List<TaxEntityReportDetail> { };
                if (searchRecords.Count() > 0)
                {
                    taxPayerReport = searchRecords.Select(x => new TaxEntityReportDetail
                    {
                        Id = x.Id,
                        Address = x.Address,
                        Category = x.TaxEntityCategory?.Name,
                        Name = x.Recipient,
                        Email = x.Email,
                        PhoneNumber = x.PhoneNumber,
                        TaxPayerIdentificationNumber = x.TaxPayerIdentificationNumber,
                        RegNumber = x.RCNumber,
                        PayerId = x.PayerId,
                        StateName = x.StateLGA.State.Name,
                        LGA = x.StateLGA.Name
                    }).ToList();
                }

                var returnObj = new TaxProfileSearchReport
                {
                    ReportRecords = taxPayerReport,
                    SearchFilter = searchFilter,
                    TotalNumberOfTaxPayers = _taxpayerReportFilter.GetAggregateForTaxProfiles(searchFilter).First().TotalNumberOfTaxProfiles,
                };

                return new APIResponse { StatusCode = System.Net.HttpStatusCode.OK, ResponseObject = returnObj };
            }
            #region catch clauses
            catch (TenantNotFoundException)
            {
                errorCode = ErrorCode.PPS1;
                errors.Add(new ErrorModel { FieldName = "ExpertSystem", ErrorMessage = "Could not find your client details" });
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                errorCode = ErrorCode.PPIE;
                errors.Add(new ErrorModel { FieldName = "SearchModel", ErrorMessage = ErrorLang.genericexception().ToString() });
            }
            #endregion
            return new APIResponse { ErrorCode = errorCode.ToString(), Error = true, StatusCode = responseCode, ResponseObject = errors };
        }

    }
}