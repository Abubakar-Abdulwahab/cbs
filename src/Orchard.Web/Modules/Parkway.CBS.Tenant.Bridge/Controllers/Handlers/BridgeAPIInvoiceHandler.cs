using Autofac;
using Newtonsoft.Json;
using Orchard;
using Orchard.Environment;
using Orchard.Environment.Configuration;
using Orchard.Environment.ShellBuilders;
using Orchard.Logging;
using Parkway.Cashflow.Ng.Models;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.StateConfig;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Module.API.Controllers.Handlers;
using Parkway.CBS.Module.API.Controllers.Handlers.Contracts;
using Parkway.CBS.RemoteClient;
using Parkway.CBS.Tenant.Bridge.Controllers.Handlers.Contracts;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;

namespace Parkway.CBS.Tenant.Bridge.Controllers.Handlers
{
    public class BridgeAPIInvoiceHandler : BaseAPIHandler, IBridgeAPIInvoiceHandler
    {
        public ILogger Logger { get; set; }
        private readonly Lazy<IInvoicingService> _invoicingService;
        private readonly IShellSettingsManager _shellSettingsManager;
        private readonly IOrchardHost _orchardHost;
        private readonly IRemoteClient _remoteClient;

        public BridgeAPIInvoiceHandler(Lazy<IInvoicingService> invoicingService, IShellSettingsManager shellSettingsManager, IOrchardHost orchardHost, IAdminSettingManager<ExpertSystemSettings> settingsRepository) : base(settingsRepository)
        {
            Logger = NullLogger.Instance;
            _invoicingService = invoicingService;
            _orchardHost = orchardHost;
            _shellSettingsManager = shellSettingsManager;
            _remoteClient = new RemoteClient.RemoteClient();
        }


        /// <summary>
        /// Validate invoice for readycash
        /// </summary>
        /// <param name="invoiceController"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public APIResponse ValidateInvoice(ReadycashInvoiceValidationModel model)
        {
            string errorMessage = null;
            try
            {
                Logger.Information(string.Format("{0}", JsonConvert.SerializeObject(model)));
                if (string.IsNullOrEmpty(model.InvoiceNumber))
                {
                    errorMessage = ErrorLang.invoice404(model.InvoiceNumber).ToString();
                    throw new Exception("No invoice found " + model.InvoiceNumber);
                }
               
                var context = _invoicingService.Value.StartInvoicingService(new Dictionary<string, dynamic> { { "companyKeyCode", "default" } });
                var invoiceService = _invoicingService.Value.InvoiceService(context);

                var responseModel = invoiceService.FetchInvoiceAndVendorCodeByNumber(model.InvoiceNumber);
                if (responseModel.HasErrors)
                {
                    errorMessage = responseModel.ResponseObject;
                    throw new Exception(errorMessage + " inv number"+ model.InvoiceNumber);
                }

                InvoiceIssuerAndVendorCode obj = responseModel.ResponseObject;
                //do tenant call for other details
                StateConfig siteConfig = GetTenantConfig(obj.VendorCode);
                Node node = siteConfig.Node.Where(x => x.Key == TenantConfigKeys.ReadycashExchangeKeyValue.ToString()).Single();
                //do work
                ShellContext shellContext = null;

                try
                {
                    shellContext = GetShellContext(siteConfig.FileSiteName);
                }
                catch (Exception)
                {
                    //Redirect the request
                    Node baseURL = siteConfig.Node.Where(x => x.Key == TenantConfigKeys.BaseURL.ToString()).Single();
                    if(baseURL == null || string.IsNullOrEmpty(baseURL.Value.Trim()))
                    {
                        throw new Exception($" Unable to find base URL for {obj.VendorCode}");
                    }

                    string validateResponse = _remoteClient.SendRequest(new RemoteClient.RequestModel
                    {
                        Headers = new Dictionary<string, dynamic> { },
                        Model = model,
                        URL = $"{baseURL.Value}{System.Web.HttpContext.Current.Request.RawUrl}",
                    }, System.Net.Http.HttpMethod.Post, new Dictionary<string, string> { });

                    Logger.Information($"{siteConfig.FileSiteName} readycash bridge invoice validation response dump: {validateResponse}");
                    return new APIResponse
                    {
                        ResponseObject = JsonConvert.DeserializeObject<ReadycashInvoiceValidationResponseModel>(validateResponse),
                        StatusCode = HttpStatusCode.OK
                    };
                }

                APIResponse response = null;

                using (var workContext = shellContext.LifetimeScope.Resolve<IWorkContextAccessor>().CreateWorkContextScope())
                {
                    response = workContext.Resolve<IAPIInvoiceHandler>().ValidateInvoice(model);
                }

                if (!response.Error && response.ResponseObject.ResponseCode == Lang.bankcollectresponseokcode.ToString())
                {
                    response.ResponseObject.ResponseDescription = obj.Issuer;
                }
                return response;
            }
            catch (Exception exception)
            {
                Logger.Information("invoice number " + model.InvoiceNumber + exception.Message);
                Logger.Error(exception, "invoice number " + model.InvoiceNumber + exception.Message);
            }

            return new APIResponse
            {
                ResponseObject = new ReadycashInvoiceValidationResponseModel
                {
                    ResponseCode = ErrorLang.bankcollecterrorcode().ToString(),
                    ResponseDescription = string.IsNullOrEmpty(errorMessage) ? ErrorLang.genericexception().ToString() : errorMessage
                },
                StatusCode = System.Net.HttpStatusCode.BadRequest
            };

        }


        /// <summary>
        /// Do invoice validation for billers
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public APIResponse BillerValidateInvoice(ReadycashInvoiceValidationModel model, dynamic headerParams)
        {
            List<ErrorModel> errors = new List<ErrorModel>();
            HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest;
            string errorCode = ResponseCodeLang.generic_exception_code;

            try
            {
                Logger.Information(string.Format("{0}", JsonConvert.SerializeObject(model)));

                StateConfig siteConfig = GetTenantConfig(headerParams.BILLERCODE);

                ShellContext shellContext = GetShellContext(siteConfig.FileSiteName);
                using (var workContext = shellContext.LifetimeScope.Resolve<IWorkContextAccessor>().CreateWorkContextScope())
                {
                    dynamic exheaderParams = new ExpandoObject();
                    exheaderParams.SIGNATURE = headerParams.SIGNATURE as string;
                    exheaderParams.CLIENTID = headerParams.CLIENTID as string;
                    return workContext.Resolve<IAPIInvoiceHandler>().InvoiceValidation(new ValidationRequest { InvoiceNumber = model.InvoiceNumber }, exheaderParams);
                }

            }
            catch (Exception exception) { Logger.Error(exception, exception.Message); }

            return new APIResponse
            {
                ResponseObject = errors,
                StatusCode = httpStatusCode,
                Error = true,
                ErrorCode = errorCode
            };
        }

        /// <summary>
        /// This handles invoice creation 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="headerParams"></param>
        /// <returns></returns>
        public APIResponse CreateInvoice(CreateInvoiceModel model, dynamic headerParams)
        {
            List<ErrorModel> errors = new List<ErrorModel>();
            HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest;
            string errorCode = ResponseCodeLang.generic_exception_code;

            try
            {
                if (model == null)
                {
                    errors.Add(new ErrorModel { ErrorMessage = ErrorLang.modelisempty().ToString(), FieldName = "Invoice" });
                    throw new Exception("Model is empty");
                }

                Logger.Information(string.Format("{0}", JsonConvert.SerializeObject(model)));

                StateConfig siteConfig = GetTenantConfig(headerParams.BILLERCODE);
                ShellContext shellContext = GetShellContext(siteConfig.FileSiteName);
                using (var workContext = shellContext.LifetimeScope.Resolve<IWorkContextAccessor>().CreateWorkContextScope())
                {
                    dynamic exheaderParams = new ExpandoObject();
                    exheaderParams.SIGNATURE = headerParams.SIGNATURE as string;
                    exheaderParams.CLIENTID = headerParams.CLIENTID as string;
                    return workContext.Resolve<IAPIInvoiceHandler>().CreateInvoice(model, exheaderParams);
                }

            }
            catch (Exception exception) { Logger.Error(exception, exception.Message); }

            return new APIResponse
            {
                ResponseObject = errors,
                StatusCode = httpStatusCode,
                Error = true,
                ErrorCode = errorCode
            };
        }


        /// <summary>
        /// Get tenant config settings
        /// </summary>
        /// <param name="vendorCode"></param>
        /// <returns>StateConfig</returns>
        private StateConfig GetTenantConfig(string vendorCode)
        {
            return Util.GetTenantConfigByVendorName(vendorCode);
        }

        /// <summary>
        /// This handles invoice validation for all the tenants 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="headerParams"></param>
        /// <returns>APIResponse</returns>
        public APIResponse InvoiceValidation(ValidationRequest model, dynamic headerParams)
        {
            List<ErrorModel> errors = new List<ErrorModel>();
            HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest;
            string errorCode = ResponseCodeLang.generic_exception_code;
            try
            {
                Logger.Information(string.Format("validate invoice for model {0}", JsonConvert.SerializeObject(new { model, headerParams })));

                var context = _invoicingService.Value.StartInvoicingService(new Dictionary<string, dynamic> { { "companyKeyCode", "default" } });
                var invoiceService = _invoicingService.Value.InvoiceService(context);
                var responseModel = invoiceService.FetchInvoiceAndVendorCodeByNumber(model.InvoiceNumber);
                if (responseModel.HasErrors)
                {
                    throw new Exception(responseModel.ResponseObject + " inv number" + model.InvoiceNumber);
                }

                InvoiceIssuerAndVendorCode obj = responseModel.ResponseObject;
                //do tenant call for other details
                StateConfig siteConfig = GetTenantConfig(obj.VendorCode);
                //do work
                ShellContext shellContext = null;
                APIResponse response = null;

                try
                {
                    shellContext = GetShellContext(siteConfig.FileSiteName);
                }
                catch (Exception)
                {
                    //Redirect the request
                    Node baseURL = siteConfig.Node.Where(x => x.Key == TenantConfigKeys.BaseURL.ToString()).Single();
                    if (baseURL == null || string.IsNullOrEmpty(baseURL.Value.Trim()))
                    {
                        throw new Exception($" Unable to find base URL for {obj.VendorCode}");
                    }

                    string validateResponse = _remoteClient.SendRequest(new RemoteClient.RequestModel
                    {
                        Headers = new Dictionary<string, dynamic> { { "SIGNATURE", headerParams.SIGNATURE }, { "CLIENTID", headerParams.CLIENTID } },
                        Model = model,
                        URL = $"{baseURL.Value}{System.Web.HttpContext.Current.Request.RawUrl}",
                    }, System.Net.Http.HttpMethod.Post, new Dictionary<string, string> { });

                    response = JsonConvert.DeserializeObject<APIResponse>(validateResponse);
                    response.StatusCode = HttpStatusCode.OK;
                    return response;
                }

                using (var workContext = shellContext.LifetimeScope.Resolve<IWorkContextAccessor>().CreateWorkContextScope())
                {
                    response = workContext.Resolve<IAPIInvoiceHandler>().InvoiceValidation(model, headerParams);
                }

                if (!response.Error && response.ResponseObject.ResponseCode == Lang.bankcollectresponseokcode.ToString())
                {
                    response.ResponseObject.ResponseDescription = obj.Issuer;
                }
                return response;

            }
            catch (UserNotAuthorizedForThisActionException)
            {
                Logger.Error("Payment provider is inactive CLIENTID " + (string)headerParams.CLIENTID);
                errors.Add(new ErrorModel { FieldName = "Invoice", ErrorMessage = ErrorLang.usernotauthorized().ToString() });
                errorCode = ResponseCodeLang.user_not_authorized;
            }
            catch (PaymentProvider404)
            {
                Logger.Error("Payment provider 404 CLIENTID " + (string)headerParams.CLIENTID);
                errors.Add(new ErrorModel { FieldName = "Provider", ErrorMessage = ErrorLang.norecord404().ToString() });
                errorCode = ResponseCodeLang.payment_provider_404;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                if(errors.Count < 1)
                {
                    throw;
                }
            }

            return new APIResponse
            {
                ResponseObject = errors,
                StatusCode = httpStatusCode,
                Error = true,
                ErrorCode = errorCode
            };
        }

        /// <summary>
        /// Get shell context for thsi operation
        /// </summary>
        /// <param name="siteName"></param>
        /// <returns>ShellContext</returns>
        public ShellContext GetShellContext(string siteName)
        {

            var tenantShellSettings = _shellSettingsManager.LoadSettings().Where(settings => settings.Name.Equals(siteName, StringComparison.InvariantCultureIgnoreCase)).Single();
            return _orchardHost.GetShellContext(tenantShellSettings);
        }

        /// <summary>
        /// Do model check
        /// </summary>
        /// <param name="callback">InvoiceBridgeController</param>
        /// <returns>List{ErrorModel}</returns>
        public List<ErrorModel> DoModelCheck(InvoiceBridgeController callback)
        {
            return CheckModelStateWithoutException(callback);
        }

    }
}