using Orchard;
using Newtonsoft.Json;
using Orchard.Logging;
using Parkway.CBS.Core.DataFilters.CBSUserTaxEntityProfileLocationReport.Contracts;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.Mail.Provider.Contracts;
using Parkway.CBS.Police.Core.Utilities;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using Parkway.CBS.Core.Services.Contracts;

namespace Parkway.CBS.Police.Client.Controllers.Handlers
{
    public class PSSSubUserHandler : IPSSSubUserHandler
    {
        private readonly IOrchardServices _orchardServices;
        private readonly ICoreUserService _coreUser;
        private readonly ICoreTaxEntityProfileLocationService _coreTaxEntityProfileLocationService;
        private readonly ICoreCBSUserTaxEntityProfileLocationService _coreCBSUserTaxEntityProfileLocationService;
        private readonly IEnumerable<Lazy<IPSSEmailProvider>> _emailProvider;
        private readonly ICBSUserTaxEntityProfileLocationFilter _cbsUserTaxEntityProfileLocationFilter;
        private readonly ICBSUserManager<CBSUser> _cbsUserManager;
        private readonly IUserRolesPartRecordManager _userRolesPartManager;
        ILogger Logger { get; set; }
        public PSSSubUserHandler(ICoreUserService coreUser, ICoreTaxEntityProfileLocationService coreTaxEntityProfileLocationService, ICoreCBSUserTaxEntityProfileLocationService coreCBSUserTaxEntityProfileLocationService, IEnumerable<Lazy<IPSSEmailProvider>> emailProvider, IOrchardServices orchardServices, ICBSUserTaxEntityProfileLocationFilter cbsUserTaxEntityProfileLocationFilter, ICBSUserManager<CBSUser> cbsUserManager, IUserRolesPartRecordManager userRolesPartManager)
        {
            _coreUser = coreUser;
            _coreTaxEntityProfileLocationService = coreTaxEntityProfileLocationService;
            _coreCBSUserTaxEntityProfileLocationService = coreCBSUserTaxEntityProfileLocationService;
            _emailProvider = emailProvider;
            _orchardServices = orchardServices;
            _cbsUserTaxEntityProfileLocationFilter = cbsUserTaxEntityProfileLocationFilter;
            _cbsUserManager = cbsUserManager;
            _userRolesPartManager = userRolesPartManager;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// Get create sub user VM
        /// </summary>
        /// <param name="taxEntityId"></param>
        /// <returns></returns>
        public PSSSubUserVM GetCreateSubUserVM(long taxEntityId)
        {
            try
            {
                return new PSSSubUserVM
                {
                    SubUserInfo = new RegisterCBSUserModel { },
                    Branches = _coreTaxEntityProfileLocationService.GetTaxEntityLocations(taxEntityId)
                };
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Creates a sub user
        /// </summary>
        /// <param name="userInput"></param>
        /// <param name="taxEntityId"></param>
        /// <param name="taxEntityCategoryId"></param>
        /// <param name="errors"></param>
        public void CreateSubUser(PSSSubUserVM userInput, long taxEntityId, int taxEntityCategoryId, ref List<ErrorModel> errors)
        {
            try
            {
                if (string.IsNullOrEmpty(userInput.SubUserInfo.Name)) { errors.Add(new ErrorModel { FieldName = "SubUserInfo.Name", ErrorMessage = "Enter a valid name." }); }
                else
                {
                    if (userInput.SubUserInfo.Name.Trim().Length < 3 || userInput.SubUserInfo.Name.Trim().Length > 100)
                    {
                        errors.Add(new ErrorModel { FieldName = "SubUserInfo.Name", ErrorMessage = "Name value should be between 3 and 100 characters." });
                    }
                }

                if (string.IsNullOrEmpty(userInput.SubUserInfo.PhoneNumber)) { errors.Add(new ErrorModel { FieldName = "SubUserInfo.PhoneNumber", ErrorMessage = "Enter a valid phone number." }); }

                if (string.IsNullOrEmpty(userInput.SubUserInfo.Email)) { errors.Add(new ErrorModel { FieldName = "SubUserInfo.Email", ErrorMessage = "Enter a valid email." }); }
                else
                {
                    if (userInput.SubUserInfo.Email.Trim().Length < 3 || userInput.SubUserInfo.Email.Trim().Length > 100)
                    {
                        errors.Add(new ErrorModel { FieldName = "SubUserInfo.Email", ErrorMessage = "Email value should be between 3 and 100 characters." });
                    }
                }

                TaxEntityProfileLocationVM location = null;
                if (userInput.SelectedBranch < 1) { errors.Add(new ErrorModel { FieldName = "SelectedBranch", ErrorMessage = "Selected branch value is not valid." }); }
                else
                {
                    location = _coreTaxEntityProfileLocationService.GetTaxEntityLocationWithId(taxEntityId, userInput.SelectedBranch);
                    if (location == null)
                    {
                        errors.Add(new ErrorModel { FieldName = "SelectedBranch", ErrorMessage = "Selected branch value is not valid." });
                    }
                }

                if (errors.Count() > 0) { throw new DirtyFormDataException(); }

                userInput.SubUserInfo.UserName = userInput.SubUserInfo.Email;
                userInput.SubUserInfo.Address = location.Address;
                userInput.SubUserInfo.SelectedState = location.State;
                userInput.SubUserInfo.SelectedStateLGA = location.LGA;

                string randomPassword = Util.GenerateRandomPassword();
                userInput.SubUserInfo.Password = randomPassword;
                userInput.SubUserInfo.ConfirmPassword = randomPassword;
                var registerUserResponse = _coreUser.TryCreateCBSSubUser(userInput.SubUserInfo, new TaxEntity { Id = taxEntityId }, new TaxEntityCategory { Id = taxEntityCategoryId }, ref errors, fieldPrefix: "SubUserInfo.", validateEmail: true, validatePhoneNumber: true);

                _coreCBSUserTaxEntityProfileLocationService.AttachUserToLocation(registerUserResponse.CBSUserId, location.Id);

                //Send email notification containing random generated password to the sub user
                SendPasswordEmailNotification(userInput);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                if (errors.Count() > 0) {
                    if (errors.Where(x => x.FieldName == "SubUserInfo.UserName").FirstOrDefault() != null)
                    {
                        string userNameMsg = errors.Where(x => x.FieldName == "SubUserInfo.UserName").FirstOrDefault().ErrorMessage;
                        errors.Add(new ErrorModel { FieldName = "SubUserInfo.Email", ErrorMessage = userNameMsg.Replace("Username","Email") });
                    }
                    throw new DirtyFormDataException();
                }
                throw;
            }
        }


        /// <summary>
        /// Sends password email notification
        /// </summary>
        /// <param name="userInput"></param>
        /// <returns></returns>
        private bool SendPasswordEmailNotification(PSSSubUserVM userInput)
        {
            dynamic emailDetails = new ExpandoObject();
            emailDetails.Email = userInput.SubUserInfo.Email;
            emailDetails.Recipient = userInput.SubUserInfo.Name.Trim();
            emailDetails.Password = userInput.SubUserInfo.Password;
            emailDetails.Subject = "POSSAP Account Creation Notification";

            if (PSSUtil.IsEmailEnabled(_orchardServices.WorkContext.CurrentSite.SiteName))
            {
                bool result = int.TryParse(AppSettingsConfigurations.GetSettingsValue(AppSettingEnum.EmailProvider), out int providerId);

                if (!result)
                {
                    providerId = (int)EmailProvider.Pulse;
                }
                foreach (var impl in _emailProvider)
                {
                    if ((EmailProvider)providerId == impl.Value.GetEmailNotificationProvider)
                    {
                        return impl.Value.PSSSubUserPasswordNotification(emailDetails);
                    }
                }
            }
            return false;
        }


        /// <summary>
        /// Gets sub users for currently logged in tax entity
        /// </summary>
        /// <param name="searchParams">search params</param>
        /// <returns>PSSBranchVM</returns>
        public PSSSubUserVM GetSubUsers(CBSUserTaxEntityProfileLocationReportSearchParams searchParams)
        {
            dynamic recordsAndAggregate = _cbsUserTaxEntityProfileLocationFilter.GetReportViewModel(searchParams);
            IEnumerable<CBSUserTaxEntityProfileLocationVM> records = ((IEnumerable<CBSUserTaxEntityProfileLocationVM>)recordsAndAggregate.ReportRecords);

            return new PSSSubUserVM
            {
                //DateFilter = string.Format("{0} - {1}", searchParams.StartDate.ToString("dd'/'MM'/'yyyy"), searchParams.EndDate.ToString("dd'/'MM'/'yyyy")),
                FilteredBranch = searchParams.Branch,
                FilteredName = searchParams.SubUserName,
                SubUsers = (records == null || !records.Any()) ? new List<CBSUserTaxEntityProfileLocationVM> { } : records.ToList(),
                TotalRecordCount = (int)(((IEnumerable<ReportStatsVM>)recordsAndAggregate.Aggregate).First().TotalRecordCount),
                Token = Util.LetsEncrypt(JsonConvert.SerializeObject(new { Branch = searchParams.Branch, StartDate = searchParams.StartDate, EndDate = searchParams.EndDate, Name = searchParams.SubUserName, ChunkSize = 10, OperatorId = searchParams.TaxEntityId }), System.Configuration.ConfigurationManager.AppSettings["EncryptionSecret"]),

            };
        }


        /// <summary>
        /// Gets Paged PSS Sub Users
        /// </summary>
        /// <param name="token">token</param>
        /// <param name="page">page</param>
        /// <param name="taxEntityId">tax entity id</param>
        /// <returns>APIResponse</returns>
        public APIResponse GetPagedSubUsersData(string token, int? page, long taxEntityId)
        {
            try
            {
                dynamic tokenModel = new ExpandoObject();
                if (!string.IsNullOrEmpty(token))
                {
                    var decryptedValue = Util.LetsDecrypt(token);
                    tokenModel = JsonConvert.DeserializeObject(decryptedValue);
                }
                else { throw new Exception("Empty Token provided in GetPagedSubUsersData for PSSSubUsersHandler"); }
                //DateTime startDate = DateTime.Now.AddMonths(-3);
                //DateTime endDate = DateTime.Now;

                //if (tokenModel.StartDate != null && tokenModel.EndDate != null)
                //{
                //    startDate = tokenModel.StartDate;
                //    endDate = tokenModel.EndDate;
                //}

                ////get the date up until the final sec
                //endDate = endDate.Date.AddDays(1).AddMilliseconds(-1);

                int skip = page.HasValue ? (page.Value - 1) * ((int)tokenModel.ChunkSize) : 0;

                CBSUserTaxEntityProfileLocationReportSearchParams searchParams = new CBSUserTaxEntityProfileLocationReportSearchParams
                {
                    TaxEntityId = taxEntityId,
                    Take = tokenModel.ChunkSize,
                    Skip = skip,
                    //EndDate = endDate,
                    //StartDate = startDate,
                    Branch = (int)tokenModel.Branch,
                    SubUserName = tokenModel.Name,
                };

                PSSSubUserVM model = GetSubUsers(searchParams);

                return new APIResponse { ResponseObject = model };
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                return new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().ToString() };
            }
        }


        /// <summary>
        /// Toggles registration status of sub user with specified user part record id
        /// </summary>
        /// <param name="subUserUserId"></param>
        /// <param name="isActive"></param>
        /// <returns></returns>
        public APIResponse ToggleSubUserStatus(int subUserUserId, bool isActive)
        {
            try
            {
                if (subUserUserId <= 0) { return new APIResponse { Error = true, ResponseObject = ErrorLang.invalidinputtype().Text }; }
                long taxEntityId = _cbsUserManager.GetTaxEntityIdForAdminCBSUserWithUserPartRecord(_orchardServices.WorkContext.CurrentUser.Id);
                if (taxEntityId < 1) { return new APIResponse { Error = true, ResponseObject = ErrorLang.usernotfound().Text }; }
                if(!_coreCBSUserTaxEntityProfileLocationService.CheckIfSubUserBelongsToAdminUser(taxEntityId, subUserUserId))
                {
                    return new APIResponse { Error = true, ResponseObject = ErrorLang.ToLocalizeString("Sub user not found").Text };
                }
                string subUserEmail = _userRolesPartManager.ToggleIsUserRegistrationStatus(subUserUserId, isActive, lastUpdatedById: _orchardServices.WorkContext.CurrentUser.Id);
                _cbsUserManager.ToggleIsActiveForCBSUserWithUserId(subUserUserId, isActive);
                return new APIResponse { ResponseObject = $"Sub user with email address {subUserEmail} has been successfully {(isActive ? "activated" : "deactivated")}" };
            }catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _cbsUserManager.RollBackAllTransactions();
                return new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().Text };
            }
        }
    }
}