using System;
using System.Collections.Generic;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models;
using Orchard.Logging;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Lang;
using Orchard.Users.Models;
using Parkway.CBS.Module.API.Controllers.Handlers.Contracts;

namespace Parkway.CBS.Module.API.Controllers.Handlers
{
    public class APIFormHandler : BaseAPIHandler, IAPIFormHandler
    {
        private readonly ICoreFormService _coreFormService;

        public APIFormHandler(ICoreFormService coreFormService, IAdminSettingManager<ExpertSystemSettings> settingsRepository) : base(settingsRepository)
        {
            _coreFormService = coreFormService;
            _settingsRepository = settingsRepository;
            Logger = NullLogger.Instance;
        }

        public APIResponse CreateFormControls(IntegrationController callback, CreateFormControls model, dynamic headerParams)
        {
            List<ErrorModel> errors = new List<ErrorModel>();
            System.Net.HttpStatusCode httpStatusCode = System.Net.HttpStatusCode.BadRequest;
            try
            {
                //get tenant settings
                ExpertSystemSettings tenant = GetExpertSystem(headerParams.CLIENTID);
                //do check for hash
                string hashString = model.RevenueHeadId + model.UserEmail + headerParams.CLIENTID;

                if (!CheckHash(hashString, headerParams.SIGNATURE, tenant.ClientSecret))
                {
                    return new APIResponse { StatusCode = System.Net.HttpStatusCode.Forbidden, Error = true, ResponseObject = new List<ErrorModel> { { new ErrorModel { ErrorMessage = ErrorLang.couldnotcomputehash().ToString(), FieldName = "Signature" } } } };
                }
                //get user
                UserPartRecord user = new UserPartRecord();
                throw new Exception("No for control exception");
                //return new APIResponse { StatusCode = System.Net.HttpStatusCode.OK, ResponseObject = _coreFormService.CreateCreateFormControls(user, ref errors) };
            }
            #region catch clauses
            catch (AuthorizedUserNotFoundException exception)
            {
                Logger.Error(exception, ErrorLang.usernotfound().ToString() + exception.Message);
                errors.Add(new ErrorModel { FieldName = "LastUpdatedBy", ErrorMessage = ErrorLang.usernotfound().ToString() });
                httpStatusCode = System.Net.HttpStatusCode.Forbidden;
            }
            catch(CannotFindRevenueHeadException exception)
            {
                Logger.Error(exception, ErrorLang.revenueheadnotfound().ToString() + exception.Message);
                errors.Add(new ErrorModel { FieldName = "RevenueHead", ErrorMessage = ErrorLang.revenueheadnotfound().ToString() });
                httpStatusCode = System.Net.HttpStatusCode.NotFound;
            }
            catch (HasNoBillingException exception)
            {
                Logger.Error(exception, ErrorLang.usernotfound().ToString() + exception.Message);
                errors.Add(new ErrorModel { FieldName = "Billing", ErrorMessage = ErrorLang.billinginfo404().ToString() });
                httpStatusCode = System.Net.HttpStatusCode.NotFound;
            }
            catch (HasFormControlsException exception)
            {
                Logger.Error(exception, ErrorLang.hasformcontrols().ToString() + exception.Message);
                errors.Add(new ErrorModel { FieldName = "FormControl", ErrorMessage = ErrorLang.hasformcontrols().ToString() });
            }
            catch (DirtyFormDataException exception)
            {
                Logger.Error(exception, exception.Message);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, ErrorLang.genericexception().ToString() + exception.Message);
                errors.Add(new ErrorModel { FieldName = "Exception", ErrorMessage = ErrorLang.genericexception().ToString() });
                httpStatusCode = System.Net.HttpStatusCode.InternalServerError;
            }
            #endregion
            return new APIResponse { Error = true, ResponseObject = errors, StatusCode = httpStatusCode };
        }
    }
}