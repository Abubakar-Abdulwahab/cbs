using Newtonsoft.Json;
using Orchard;
using Orchard.Logging;
using Parkway.CBS.Core;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Payee;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Module.API.Controllers.Handlers.Contracts;
using Parkway.CBS.Services.PAYEAPI;
using Parkway.CBS.Services.PAYEAPI.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace Parkway.CBS.Module.API.Controllers.Handlers
{
    public class APIPAYEValidationHandler : BaseAPIHandler, IAPIPAYEValidationHandler
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IPAYEAPIRequestManager<PAYEAPIRequest> _payeAPIRequestManager;
        private readonly ICorePAYEService _corePAYEService;


        public APIPAYEValidationHandler(IAdminSettingManager<ExpertSystemSettings> settingsRepository, IOrchardServices orchardServices, IPAYEAPIRequestManager<PAYEAPIRequest> payeAPIRequestManager, ICorePAYEService corePAYEService) : base(settingsRepository)
        {
            _settingsRepository = settingsRepository;
            Logger = NullLogger.Instance;
            _orchardServices = orchardServices;
            _payeAPIRequestManager = payeAPIRequestManager;
            _corePAYEService = corePAYEService;
        }

        /// <summary>
        /// Validate batch items for a specified batch identifier
        /// </summary>
        /// <param name="model"></param>
        /// <param name="headerParams"></param>
        /// <returns>APIResponse</returns>
        public APIResponse ValidateBatchItem(PAYEValidateBatchModel model, dynamic headerParams = null)
        {
            List<ErrorModel> errors = new List<ErrorModel>();
            HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest;
            string errorCode = ErrorCode.PPIE.ToString();

            try
            {
                if (model == null)
                {
                    errors.Add(new ErrorModel { FieldName = "PAYEValidation", ErrorMessage = ErrorLang.valuerequired("BatchIdentifier").ToString() });
                    throw new Exception("Model is empty");
                }
                Logger.Information(string.Format("validate PAYE batch item for model {0}", JsonConvert.SerializeObject(new { model, headerParams })));

                //get tenant settings
                ExpertSystemSettings expertSystem = GetExpertSystem(headerParams.CLIENTID);

                //get the batch info
                PAYEAPIRequestBatchDetailVM batchDetailVM = _payeAPIRequestManager.GetBatchDetails(model.BatchIdentifier, expertSystem.Id);


                string hashString = model.BatchIdentifier.ToString() + batchDetailVM.PayerId;

                if (!CheckHash(hashString, headerParams.SIGNATURE, expertSystem.ClientSecret))
                {
                    Logger.Error("Could not compute signature hash " + hashString);
                    return new APIResponse { StatusCode = HttpStatusCode.Forbidden, Error = true, ResponseObject = new List<ErrorModel> { { new ErrorModel { ErrorMessage = ErrorLang.couldnotcomputehash().ToString(), FieldName = "Signature" } } } };
                }

                //Queue the job to handle the validation of the batch
                try
                {
                    //Passed to the hangfire
                    //getting adapter
                    AssessmentInterface adapter = _corePAYEService.GetDirectAssessmentAdapter(batchDetailVM.AdapterValue);

                    string siteName = _orchardServices.WorkContext.CurrentSite.SiteName;
                    IPAYEBatchItemValidation batch = new PAYEBatchItemValidation();
                    batch.ProcessPAYEBatchItemsValidation(siteName.Replace(" ", ""), model.BatchIdentifier, expertSystem.Id, adapter);

                    return new APIResponse { StatusCode = HttpStatusCode.OK, ResponseObject = new { Message = Lang.payebatchvalidation.ToString(), batchDetailVM.BatchIdentifier } };
                }
                #region catch clauses
                catch (Exception exception)
                {
                    Logger.Error(exception, exception.Message);
                    errors.Add(new ErrorModel { FieldName = "Model", ErrorMessage = ErrorLang.genericexception().ToString() });
                }
                #endregion
                
            }
            #region catch clauses
            catch (TenantNotFoundException exception)
            {
                Logger.Error(exception, ErrorLang.tenant404("Tenant").ToString() + exception.Message);
                errors.Add(new ErrorModel { FieldName = "Tenant", ErrorMessage = ErrorLang.tenant404("Tenant").ToString() });
                httpStatusCode = HttpStatusCode.NotFound;
                errorCode = ErrorCode.PPTENANT404.ToString();
            }
            catch (NoRecordFoundException)
            {
                Logger.Error("PAYE batch identifier 404 " + model.BatchIdentifier);
                errors.Add(new ErrorModel { FieldName = "BatchIdentifier", ErrorMessage = ErrorLang.norecord404().ToString() });
                httpStatusCode = HttpStatusCode.NotFound;
                errorCode = ErrorCode.PPPY404.ToString();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                errors.Add(new ErrorModel { FieldName = "Model", ErrorMessage = ErrorLang.genericexception().ToString() });
            }
            #endregion

            return new APIResponse
            {
                ResponseObject = errors,
                StatusCode = httpStatusCode,
                Error = true,
                ErrorCode = errorCode
            };
        }
    }
}