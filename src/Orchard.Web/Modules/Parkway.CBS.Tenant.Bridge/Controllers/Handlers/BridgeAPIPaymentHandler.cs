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
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.StateConfig;
using Parkway.CBS.Core.Utilities;
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
    public class BridgeAPIPaymentHandler : IBridgeAPIPaymentHandler
    {

        public ILogger Logger { get; set; }
        private readonly IInvoicingService _invoicingService;
        private readonly IShellSettingsManager _shellSettingsManager;
        private readonly IOrchardHost _orchardHost;
        private readonly IRemoteClient _remoteClient;

        public BridgeAPIPaymentHandler(IInvoicingService invoicingService, IShellSettingsManager shellSettingsManager, IOrchardHost orchardHost)
        {
            Logger = NullLogger.Instance;
            _invoicingService = invoicingService;
            _orchardHost = orchardHost;
            _shellSettingsManager = shellSettingsManager;
            _remoteClient = new RemoteClient.RemoteClient();
        }


        public APIResponse PaymentNotification(ReadyCashPaymentNotification model)
        {
            string errorMessage = null;
            try
            {
                Logger.Information(string.Format("{0}", JsonConvert.SerializeObject(model)));
                if (string.IsNullOrEmpty(model.InvoiceNumber))
                {
                    errorMessage = ErrorLang.invoice404(model.InvoiceNumber).ToString();
                    throw new Exception("No invoice found");
                }

                var context = _invoicingService.StartInvoicingService(new Dictionary<string, dynamic> { { "companyKeyCode", "default" } });
                var invoiceService = _invoicingService.InvoiceService(context);

                var responseModel = invoiceService.FetchInvoiceAndVendorCodeByNumber(model.InvoiceNumber);

                if (responseModel.HasErrors)
                {
                    errorMessage = responseModel.ResponseObject;
                    throw new Exception(errorMessage);
                }

                InvoiceIssuerAndVendorCode obj = responseModel.ResponseObject;
                //do tenant call for other details
                StateConfig siteConfig = Util.GetTenantConfigByVendorName(obj.VendorCode);
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
                    if (baseURL == null || string.IsNullOrEmpty(baseURL.Value.Trim()))
                    {
                        throw new Exception($" Unable to find base URL for {obj.VendorCode}");
                    }

                    string paymentResponse = _remoteClient.SendRequest(new RemoteClient.RequestModel
                    {
                        Headers = new Dictionary<string, dynamic> { },
                        Model = model,
                        URL = $"{baseURL.Value}{System.Web.HttpContext.Current.Request.RawUrl}",
                    }, System.Net.Http.HttpMethod.Post, new Dictionary<string, string> { });

                    Logger.Information($"{siteConfig.FileSiteName} readycash bridge payment notification response dump: {paymentResponse}");
                    return new APIResponse
                    {
                        ResponseObject = JsonConvert.DeserializeObject<ReadyCashPaymentNotificationResponseModel>(paymentResponse),
                        StatusCode = HttpStatusCode.OK
                    };
                }

                APIResponse response = null;

                using (var workContext = shellContext.LifetimeScope.Resolve<IWorkContextAccessor>().CreateWorkContextScope())
                {
                    response = workContext.Resolve<IAPIPaymentHandler>().PaymentNotification(model);
                }
                return response;
            }
            catch (Exception exception) { Logger.Error(exception, "Invoice number " + model.InvoiceNumber  + exception.Message); }

            return new APIResponse
            {
                ResponseObject = new ReadycashInvoiceValidationResponseModel
                {
                    ResponseCode = ErrorLang.bankcollecterrorcode().ToString(),
                    ResponseDescription = string.IsNullOrEmpty(errorMessage) ? ErrorLang.genericexception().ToString() : errorMessage
                },
                StatusCode = System.Net.HttpStatusCode.OK
            };
        }



        /// <summary>
        /// Process payment notifications
        /// </summary>
        /// <param name="model"></param>
        /// <returns>APIResponse</returns>
        public APIResponse ProcessPaymentNotification(PaymentNotification model, dynamic headerParams)
        {
            List<ErrorModel> errors = new List<ErrorModel>();
            HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest;
            string errorCode = ResponseCodeLang.generic_exception_code;

            try
            {
                Logger.Information(string.Format("{0}", JsonConvert.SerializeObject(model)));
                if (model == null || string.IsNullOrEmpty(model.InvoiceNumber))
                {
                    errors.Add(new ErrorModel { ErrorMessage = ErrorLang.valuerequired("Invoice").ToString(), FieldName = "Invoice" });
                    throw new Exception("Model is empty");
                }

                StateConfig siteConfig = GetTenantConfig(headerParams.BILLERCODE);

                ShellContext shellContext = GetShellContext(siteConfig.FileSiteName);
                using (var workContext = shellContext.LifetimeScope.Resolve<IWorkContextAccessor>().CreateWorkContextScope())
                {
                    dynamic exheaderParams = new ExpandoObject();
                    exheaderParams.SIGNATURE = headerParams.SIGNATURE as string;
                    exheaderParams.CLIENTID = headerParams.CLIENTID as string;
                    return workContext.Resolve<IAPIPaymentHandler>().PaymentNotification(model, exheaderParams);
                }

            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                if(errors.Count < 1) { errors.Add(new ErrorModel { ErrorMessage = ErrorLang.genericexception().ToString(), FieldName = "Invoice"  }); }
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
        /// Check for transaction with the ref and channel for this payment provider
        /// </summary>
        /// <param name="model"></param>
        /// <param name="p"></param>
        /// <returns>APIResponse</returns>
        public APIResponse RequeryTransaction(PaymentNotification model, dynamic headerParams)
        {
            List<ErrorModel> errors = new List<ErrorModel>();
            HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest;
            string errorCode = ResponseCodeLang.generic_exception_code;

            try
            {
                Logger.Information(string.Format("{0}", JsonConvert.SerializeObject(model)));
                if (model == null || string.IsNullOrEmpty(model.PaymentRef))
                {
                    errors.Add(new ErrorModel { ErrorMessage = ErrorLang.valuerequired("Payment").ToString(), FieldName = "Payment" });
                    throw new Exception("Model is empty");
                }

                StateConfig siteConfig = GetTenantConfig(headerParams.BILLERCODE);

                ShellContext shellContext = GetShellContext(siteConfig.FileSiteName);
                using (var workContext = shellContext.LifetimeScope.Resolve<IWorkContextAccessor>().CreateWorkContextScope())
                {
                    dynamic exheaderParams = new ExpandoObject();
                    exheaderParams.SIGNATURE = headerParams.SIGNATURE as string;
                    exheaderParams.CLIENTID = headerParams.CLIENTID as string;
                    return workContext.Resolve<IAPIPaymentHandler>().GetReadycashTransactionRequery(model, exheaderParams);
                }

            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                if (errors.Count < 1) { errors.Add(new ErrorModel { ErrorMessage = ErrorLang.genericexception().ToString(), FieldName = "Invoice" }); }
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
        /// This handles payment notification for all the tenants 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="headerParams"></param>
        /// <returns>APIResponse</returns>
        public APIResponse GenericePaymentNotification(PaymentNotification model, dynamic headerParams)
        {
            List<ErrorModel> errors = new List<ErrorModel>();
            HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest;
            string errorCode = ResponseCodeLang.generic_exception_code;

            try
            {
                if (model == null)
                {
                    errors.Add(new ErrorModel { FieldName = "Payment", ErrorMessage = ErrorLang.valuerequired("Invoice").ToString() });
                    throw new Exception("Model is empty");
                }
                Logger.Information(string.Format("Payment notification request for model {0}", JsonConvert.SerializeObject(new { model, headerParams })));

                var context = _invoicingService.StartInvoicingService(new Dictionary<string, dynamic> { { "companyKeyCode", "default" } });
                var invoiceService = _invoicingService.InvoiceService(context);
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

                    string paymentResponse = _remoteClient.SendRequest(new RemoteClient.RequestModel
                    {
                        Headers = new Dictionary<string, dynamic> { { "SIGNATURE", headerParams.SIGNATURE }, { "CLIENTID", headerParams.CLIENTID } },
                        Model = model,
                        URL = $"{baseURL.Value}{System.Web.HttpContext.Current.Request.RawUrl}",
                    }, System.Net.Http.HttpMethod.Post, new Dictionary<string, string> { });

                    response = JsonConvert.DeserializeObject<APIResponse>(paymentResponse);
                    response.StatusCode = HttpStatusCode.OK;
                    return response;
                }

                using (var workContext = shellContext.LifetimeScope.Resolve<IWorkContextAccessor>().CreateWorkContextScope())
                {
                    response = workContext.Resolve<IAPIPaymentHandler>().PaymentNotification(model, headerParams);
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
            catch (NoRecordFoundException)
            {
                errors.Add(new ErrorModel { ErrorMessage = ErrorLang.datamismatch().ToString(), FieldName = "Payment" });
                errorCode = ResponseCodeLang.payment_data_mismatch;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                if (errors.Count < 1)
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
        /// Get tenant config settings
        /// </summary>
        /// <param name="vendorCode"></param>
        /// <returns>StateConfig</returns>
        private StateConfig GetTenantConfig(string vendorCode)
        {
            return Util.GetTenantConfigByVendorName(vendorCode);
        }


        /// <summary>
        /// Get shell context for thsi operation
        /// </summary>
        /// <param name="siteName"></param>
        /// <returns>ShellContext</returns>
        protected ShellContext GetShellContext(string siteName)
        {
            var tenantShellSettings = _shellSettingsManager.LoadSettings().Where(settings => settings.Name.Equals(siteName, StringComparison.InvariantCultureIgnoreCase)).Single();
            return _orchardHost.GetShellContext(tenantShellSettings);
        }

    }
}