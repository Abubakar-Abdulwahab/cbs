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
using Parkway.CBS.Core.StateConfig;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Utilities;
using Orchard;
using Parkway.CBS.Core.SMS.Provider.Contracts;
using System.Dynamic;

namespace Parkway.CBS.Module.API.Controllers.Handlers
{
    public class APIStateTINHandler : BaseAPIHandler, IAPIStateTINHandler
    {
        private readonly ITaxEntityCategoryManager<TaxEntityCategory> _taxCategoryManager;
        private readonly ICoreUserService _coreUserService;
        private readonly IStateModelManager<StateModel> _stateModelManager;
        private readonly ILGAManager<LGA> _lgaManager;
        private readonly ITaxEntityManager<TaxEntity> _taxEntityManager;
        private readonly IOrchardServices _orchardServices;
        private readonly IEnumerable<ISMSProvider> _smsProvider;



        public APIStateTINHandler(IAdminSettingManager<ExpertSystemSettings> settingsRepository, ITaxEntityCategoryManager<TaxEntityCategory> taxCategoryManager, ICoreUserService coreUserService, IStateModelManager<StateModel> stateModelManager, ILGAManager<LGA> lgaManager, ITaxEntityManager<TaxEntity> taxEntityManager, IOrchardServices orchardServices, IEnumerable<ISMSProvider> smsProvider) : base(settingsRepository)
        {
            _orchardServices = orchardServices;
            _smsProvider = smsProvider;
            _taxCategoryManager = taxCategoryManager;
            _settingsRepository = settingsRepository;
            _coreUserService = coreUserService;
            _stateModelManager = stateModelManager;
            _lgaManager = lgaManager;
            _taxEntityManager = taxEntityManager;
            Logger = NullLogger.Instance;
        }

        /// <summary>
        /// Create a CBS user
        /// </summary>
        /// <param name="callback">UserController</param>
        /// <param name="model">RegisterCBSUserModel</param>
        /// <param name="headerParams">dynamic</param>
        /// <returns>APIResponse</returns>
        public APIResponse CreateStateTIN(CreateStateTINModel model, dynamic headerParams)
        {
            List<ErrorModel> errors = new List<ErrorModel>();
            System.Net.HttpStatusCode responseCode = System.Net.HttpStatusCode.BadRequest;
            ErrorCode errorCode = new ErrorCode();
            if (model == null)
            {
                errors.Add(new ErrorModel { ErrorMessage = ErrorLang.modelisempty().ToString(), FieldName = "StateTIN" });
                return new APIResponse { Error = true, ErrorCode = ErrorCode.PPVE.ToString(), ResponseObject = errors, StatusCode = System.Net.HttpStatusCode.BadRequest };
            }

            Logger.Error("Creating CBS user for model " + JsonConvert.SerializeObject(model));
            try
            {
                ////get tenant settings
                ExpertSystemSettings expertSystem = GetExpertSystem(headerParams.CLIENTID);
                //do check for hash
                string value = model.PhoneNumber + model.PayerCategory + model.StateCode + model.LGACode + headerParams.CLIENTID;
                if (!CheckHash(value, headerParams.SIGNATURE, expertSystem.ClientSecret))
                {
                    Logger.Error("Signature hash does not match " + value);
                    return new APIResponse { ErrorCode = ErrorCode.PPS1.ToString(), StatusCode = System.Net.HttpStatusCode.Forbidden, Error = true, ResponseObject = new List<ErrorModel> { { new ErrorModel { ErrorMessage = ErrorLang.couldnotcomputehash().ToString(), FieldName = "Signature" } } } };
                }

                TaxEntityCategory category = _taxCategoryManager.Get(cato => cato.Id == model.PayerCategory);
                if (category == null) { throw new NoCategoryFoundException(); }
                IEnumerable<LGAVM> lga = _lgaManager.GetLGA(model.LGACode);
                if (lga == null || lga.Count() < 1) { throw new LGANotFoundException(); }
                if (lga.First().StateShortName != model.StateCode) { throw new StateNotFoundException(); }

                string defaultPassword = Util.GenerateRandomPassword();
                var result = _coreUserService.TryCreateCBSUser(
                    new RegisterCBSUserModel
                    {
                        Address = model.Address,
                        CategoryIdentifier = model.PayerCategory,
                        Password = defaultPassword,
                        ConfirmPassword = defaultPassword,
                        Email = model.Email,
                        Name = model.Name,
                        PhoneNumber = model.PhoneNumber,
                        UserName = string.IsNullOrEmpty(model.Email) ? model.PhoneNumber : model.Email,
                        SelectedState = lga.First().StateId,
                        SelectedStateLGA = lga.First().Id,
                    }, category, ref errors, expertSystem, validatePhoneNumber: true);

                if (errors.Count > 0) { throw new DirtyFormDataException(); }

                SendStateTINBySMS(model.Name, model.PhoneNumber, result.TaxEntityVM.PayerId);
                APIResponse response = new APIResponse
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    ResponseObject = new CreateStateTINResponse
                    {
                        Name = model.Name,
                        PhoneNumber = model.PhoneNumber,
                        StateTIN = result.TaxEntityVM.PayerId,
                        NormalizedStateTIN = result.TaxEntityVM.PayerId.Split('-')[1]
                    }
                };
                Logger.Information(string.Format("StateTINResponse {0}", JsonConvert.SerializeObject(response)));
                return response;
            }
            #region catch clauses
            catch (TenantNotFoundException)
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
            catch (LGANotFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                errorCode = ErrorCode.PPREC404;
                errors.Add(new ErrorModel { FieldName = "LGA", ErrorMessage = ErrorLang.lganotfound().ToString() });
            }
            catch (StateNotFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                errorCode = ErrorCode.PPREC404;
                errors.Add(new ErrorModel { FieldName = "STATE", ErrorMessage = ErrorLang.statenotfound().ToString() });
            }
            catch (CBSUserAlreadyExistsException)
            {
                errorCode = ErrorCode.PPUSERALREADYEXISTS;
                errors.Add(new ErrorModel { ErrorMessage = ErrorLang.profilealreadyexists().ToString(), FieldName = "TIN" });
            }
            catch (PhoneNumberHasBeenTakenException exception)
            {
                Logger.Error(exception, exception.Message);
            }
            catch (DirtyFormDataException exception)
            {
                Logger.Error(exception, exception.Message);
                errorCode = ErrorCode.PPVE;
                errors.Add(new ErrorModel { ErrorMessage = ErrorLang.genericexception().ToString(), FieldName = "Form" });
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
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                errorCode = ErrorCode.PPIE;
                errors.Add(new ErrorModel { FieldName = "StateTIN", ErrorMessage = ErrorLang.genericexception().ToString() });
            }
            #endregion
            return new APIResponse { ErrorCode = errorCode.ToString(), Error = true, StatusCode = responseCode, ResponseObject = errors };
        }


        private bool SendStateTINBySMS(string payerName, string payerPhoneNumber, string stateTIN)
        {
            try
            {
                Node node = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName)
                    .Node.Where(x => x.Key == TenantConfigKeys.IsSMSEnabled.ToString()).FirstOrDefault();
                if (node != null && !string.IsNullOrEmpty(node.Value))
                {
                    bool isSMSEnabled = false;
                    bool.TryParse(node.Value, out isSMSEnabled);
                    if (isSMSEnabled && !string.IsNullOrEmpty(payerPhoneNumber))
                    {
                        //Send sms notification
                        int providerId = 0;
                        bool response = Int32.TryParse(AppSettingsConfigurations.GetSettingsValue(AppSettingEnum.SMSProvider), out providerId);
                        if (!response)
                        {
                            providerId = (int)SMSProvider.Pulse;
                        }
                        foreach (var impl in _smsProvider)
                        {
                            if ((SMSProvider)providerId == impl.GetSMSNotificationProvider)
                            {
                                string message = $"Dear {payerName}, your STATE TIN is {stateTIN}, kindly use this when making future payments.";
                                impl.SendSMS(new List<string> { payerPhoneNumber }, message, _orchardServices.WorkContext.CurrentSite.SiteName);
                                return true;
                            }
                        }
                    }
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Do model check
        /// </summary>
        /// <param name="callback">StateTINController</param>
        /// <returns>List{ErrorModel}</returns>
        public List<ErrorModel> DoModelCheck(StateTINController callback)
        {
            return CheckModelStateWithoutException(callback);
        }

    }
}