using Orchard.Logging;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Core;
using Orchard.Users.Models;
using Newtonsoft.Json;
using Parkway.CBS.Module.API.Controllers.Handlers.Contracts;

namespace Parkway.CBS.Module.API.Controllers.Handlers
{
    public class APIMDAHandler : BaseAPIHandler, IAPIMDAHandler
    {
        private readonly ICoreMDAService _coreMDAService;
        private readonly IInvoicingService _invoicingService;

        public APIMDAHandler(ICoreMDAService coreMDAService, IAdminSettingManager<ExpertSystemSettings> settingsRepository, IInvoicingService invoicingService) : base(settingsRepository)
        {
            _coreMDAService = coreMDAService;
            Logger = NullLogger.Instance;
            _invoicingService = invoicingService;
        }

        /// <summary>
        /// Create an MDA
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="model"></param>
        /// <param name="files"></param>
        /// <param name="headerParams"></param>
        /// <returns>APIResponse <see cref="APIResponse"/></returns>       
        public APIResponse CreateMDA(MDAController callback, CreateMDAModel model, HttpFileCollectionWrapper files, dynamic headerParams = null)
        {
            List<ErrorModel> errors = new List<ErrorModel>();
            System.Net.HttpStatusCode responseCode = System.Net.HttpStatusCode.BadRequest;
            ErrorCode errorCode = new ErrorCode();            
            try
            {
                //check if model state is valid
                callback.Validate(model);
                CheckModelState<MDAController>(callback, ref errors);
                //get tenant settings
                ExpertSystemSettings expertSystem = GetExpertSystem(headerParams.CLIENTID);

                //do check for hash
                string value = model.MDA.BankDetails.BankAccountNumber + model.MDA.BankDetails.BankId;
                Logger.Error(string.Format("Validating create MDA API request. Tenant gotten {0} {1} hash value: {2}", expertSystem.ClientId, expertSystem.TenantCBSSettings.StateName, value));
                if (!CheckHash(value, headerParams.SIGNATURE, expertSystem.ClientSecret))
                {
                    Logger.Error(string.Format("Signature ver failed for create MDA request Tenant {0} {1} model: {2} ", expertSystem.ClientId,expertSystem.TenantCBSSettings.StateName, JsonConvert.SerializeObject(model)));
                    return new APIResponse { ErrorCode = ErrorCode.PPS1.ToString(), StatusCode = System.Net.HttpStatusCode.BadRequest, Error = true, ResponseObject = new List<ErrorModel> { { new ErrorModel { ErrorMessage = ErrorLang.couldnotcomputehash().ToString(), FieldName = "Signature" } } } };
                }
                //get bank TaxEntityId
                model.MDA.BankDetails.BankCode = GetBankCode(model.MDA.BankDetails.BankId);
                UserPartRecord user = GetUser(model.UserEmail);
                Logger.Error(string.Format("Signature Ver done, saving record"));
                var result = _coreMDAService.TrySaveMDA(expertSystem, model.MDA, user, ref errors, files, model.RequestIdentifier);
                Logger.Error(string.Format("{0} has created MDA {1}", callback.Request.RequestUri.AbsoluteUri, result.CBSId));
                return new APIResponse { StatusCode = System.Net.HttpStatusCode.OK, ResponseObject = result };
            }
            #region Catch clauses
            catch (TenantNotFoundException exception)
            {
                Logger.Error(exception, string.Format("Exception - {0}", exception.Message));
                errorCode = ErrorCode.PPTENANT404;
                errors.Add(new ErrorModel { FieldName = "Tenant", ErrorMessage = ErrorLang.tenant404().ToString() });
            }
            catch (NoBankDetailsOnCashflowFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                errorCode = ErrorCode.PPB1;
                errors.Add(new ErrorModel { FieldName = "BankName", ErrorMessage = ErrorLang.bank404(model.MDA.BankDetails.BankId).ToString() });
            }
            catch (DirtyFormDataException exception)
            {
                Logger.Error(exception, string.Format("Error occured while saving MDA from API - MDA - {0} Exception - {1}", Util.SimpleDump(model.MDA), exception.Message));
                errorCode = ErrorCode.PPVE;
            }
            catch (CannotSaveMDARecordException exception)
            {
                Logger.Error(exception, string.Format("Error occured while saving MDA from API - MDA - {0} Exception - {1}", Util.SimpleDump(model.MDA), exception.Message));
                errorCode = ErrorCode.PPM1;
                errors.Add(new ErrorModel { FieldName = "MDA", ErrorMessage = ErrorLang.couldnotsavemdarecord().ToString() });
            }
            catch (CannotConnectToCashFlowException exception)
            {
                Logger.Error(exception, string.Format("Cannot connect to invoicing service at the moment. MDA - {0}, Exception - {1}", Util.SimpleDump(model.MDA), exception.Message));
                errorCode = ErrorCode.PPC1;
                errors.Add(new ErrorModel { FieldName = "MDA", ErrorMessage = ErrorLang.cannotconnettoinvoicingservice().ToString() });
            }
            catch (AuthorizedUserNotFoundException exception)
            {
                Logger.Error(exception, ErrorLang.usernotfound().ToString());
                errorCode = ErrorCode.PPUSER404;
                errors.Add(new ErrorModel { FieldName = "User", ErrorMessage = ErrorLang.usernotfound().ToString() });
            }            
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Error occured while saving MDA from API - MDA - {0} Exception - {1}", Util.SimpleDump(model.MDA), exception.Message));
                errorCode = ErrorCode.PPIE;
                errors.Add(new ErrorModel { FieldName = "MDA", ErrorMessage = ErrorLang.couldnotsavemdarecord().ToString() });
            }
            #endregion
            return new APIResponse {Error = true, ErrorCode = errorCode.ToString(), StatusCode = responseCode, ResponseObject = errors };
        }



        public APIResponse EditMDA(MDAController callback, EditMDAModel model, HttpFileCollectionWrapper files, dynamic headerParams = null)
        {
            List<ErrorModel> errors = new List<ErrorModel>();
            System.Net.HttpStatusCode responseCode = System.Net.HttpStatusCode.BadRequest;
            ErrorCode errorCode = new ErrorCode();
            try
            {
                //check if model state is valid
                callback.Validate(model);
                CheckModelState<MDAController>(callback, ref errors);
                //get tenant settings
                ExpertSystemSettings tenant = GetExpertSystem(headerParams.CLIENTID);
                //do check for hash
                string value = model.Name + model.Code + model.AdditionalBankAccount.BankAccountNumber + model.UserEmail + tenant.ClientId;
                if (!CheckHash(value, headerParams.SIGNATURE, tenant.ClientSecret))
                {
                    return new APIResponse { ErrorCode = ErrorCode.PPS1.ToString(), StatusCode = System.Net.HttpStatusCode.BadRequest, Error = true, ResponseObject = new List<ErrorModel> { { new ErrorModel { ErrorMessage = ErrorLang.couldnotcomputehash().ToString(), FieldName = "Signature" } } } };
                }
                //get bank BankCode
                model.AdditionalBankAccount.BankCode = GetBankCode(model.AdditionalBankAccount.BankId);
                var user = GetUser(model.UserEmail);
                MDA updatedMDA = CreateUpdatedModel(model);
                
                var result = _coreMDAService.TryUpdate(tenant, updatedMDA, model.Id, user, ref errors, files);
                return new APIResponse { StatusCode = System.Net.HttpStatusCode.OK, ResponseObject = result };
            }
            #region Catch clauses
            catch (TenantNotFoundException exception)
            {
                Logger.Error(exception, string.Format("Exception - {0}", exception.Message));
                errorCode = ErrorCode.PPTENANT404;
                errors.Add(new ErrorModel { FieldName = "Tenant", ErrorMessage = ErrorLang.tenant404().ToString() });
            }
            catch (MDARecordNotFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                errorCode = ErrorCode.PPM404;
                errors.Add(new ErrorModel { FieldName = "MDA", ErrorMessage = ErrorLang.mdacouldnotbefound().ToString() });
            }
            catch (NoBankDetailsOnCashflowFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                errorCode = ErrorCode.PPB1;
                errors.Add(new ErrorModel { FieldName = "BankName", ErrorMessage = ErrorLang.bank404(model.AdditionalBankAccount.BankId).ToString() });
            }
            catch (DirtyFormDataException exception)
            {
                Logger.Error(exception, string.Format("Error occured while saving MDA from API - MDA - {0} Exception - {1}", Util.SimpleDump(model), exception.Message));
                errorCode = ErrorCode.PPVE;
            }
            catch (CannotSaveMDARecordException exception)
            {
                Logger.Error(exception, string.Format("Error occured while saving MDA from API - MDA - {0} Exception - {1}", Util.SimpleDump(model), exception.Message));
                errorCode = ErrorCode.PPM1;
                errors.Add(new ErrorModel { FieldName = "MDA", ErrorMessage = ErrorLang.couldnotsavemdarecord().ToString() });
            }
            catch (CannotConnectToCashFlowException exception)
            {
                Logger.Error(exception, string.Format("Cannot connect to invoicing service at the moment. MDA - {0}, Exception - {1}", Util.SimpleDump(model), exception.Message));
                errorCode = ErrorCode.PPC1;
                errors.Add(new ErrorModel { FieldName = "MDA", ErrorMessage = ErrorLang.cannotconnettoinvoicingservice().ToString() });
            }
            catch (AuthorizedUserNotFoundException exception)
            {
                Logger.Error(exception, ErrorLang.usernotfound().ToString());
                errorCode = ErrorCode.PPUSER404;
                errors.Add(new ErrorModel { FieldName = "User", ErrorMessage = ErrorLang.usernotfound().ToString() });
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Error occured while saving MDA from API - MDA - {0} Exception - {1}", Util.SimpleDump(model), exception.Message));
                errorCode = ErrorCode.PPIE;
                errors.Add(new ErrorModel { FieldName = "MDA", ErrorMessage = ErrorLang.couldnotsavemdarecord().ToString() });
            }
            #endregion
            return new APIResponse { Error = true, ErrorCode = errorCode.ToString(), StatusCode = responseCode, ResponseObject = errors };
        }

        private MDA CreateUpdatedModel(EditMDAModel model)
        {
            return new MDA
            {
                Name = model.Name,
                Code = model.Code,
                BankDetails = model.AdditionalBankAccount,
                MDASettings = new MDASettings
                {
                    CompanyAddress = model.CompanyAddress,
                    CompanyEmail = model.CompanyEmail,
                    CompanyMobile = model.CompanyMobilePhoneNumber,
                    BusinessNature = model.BusinessNature,
                },
            };
        }

        /// <summary>
        /// Get bank Id from bank name
        /// </summary>
        /// <param name="bankName"></param>
        /// <returns>int</returns>
        private string GetBankCode(int bankId)
        {
            try
            {
                #region CASHFLOW 
                var context = _invoicingService.StartInvoicingService(new Dictionary<string, dynamic> { { "companyKeyCode", "" } });
                var bankService = _invoicingService.BankService(context);
                var listOfBanks = bankService.ListOfBanks();
                return listOfBanks.Where(b => b.Id == bankId).Single().Code;
                #endregion
            }
            catch (Exception) { throw new NoBankDetailsOnCashflowFoundException(); }
        }
    }    
}