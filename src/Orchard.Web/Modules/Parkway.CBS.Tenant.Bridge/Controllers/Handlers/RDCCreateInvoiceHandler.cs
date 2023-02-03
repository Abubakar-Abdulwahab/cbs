using System;
using Orchard;
using Autofac;
using System.Net;
using System.Linq;
using System.Dynamic;
using Newtonsoft.Json;
using Orchard.Logging;
using Orchard.Environment;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Utilities;
using System.Collections.Generic;
using Parkway.CBS.Core.StateConfig;
using Parkway.CBS.Core.HelperModels;
using Orchard.Environment.Configuration;
using Orchard.Environment.ShellBuilders;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Module.API.Controllers.Handlers.Contracts;
using Parkway.CBS.Tenant.Bridge.Controllers.Handlers.Contracts;


namespace Parkway.CBS.Tenant.Bridge.Controllers.Handlers
{
    public class RDCCreateInvoiceHandler : IRDCCreateInvoiceHandler
    {

        public ILogger Logger { get; set; }
        private readonly Lazy<IInvoicingService> _invoicingService;
        private readonly IShellSettingsManager _shellSettingsManager;
        private readonly IOrchardHost _orchardHost;

        public RDCCreateInvoiceHandler(Lazy<IInvoicingService> invoicingService, IShellSettingsManager shellSettingsManager, IOrchardHost orchardHost)
        {
            Logger = NullLogger.Instance;
            _invoicingService = invoicingService;
            _orchardHost = orchardHost;
            _shellSettingsManager = shellSettingsManager;
        }



        public APIResponse GenerateInvoice(RDCBillerCreateInvoiceModel model, dynamic headerParams)
        {
            List<ErrorModel> errors = new List<ErrorModel>();
            HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest;
            string errorCode = ResponseCodeLang.generic_exception_code;

            try
            {
                if(model == null)
                {
                    errors.Add(new ErrorModel { ErrorMessage = ErrorLang.invoice404(model.TaxPayerCode).ToString(), FieldName = "Invoice" });
                    throw new Exception("Model is empty");
                }

                Logger.Information(string.Format("{0}", JsonConvert.SerializeObject(model)));

                if (string.IsNullOrEmpty(model.PhoneNumber) || string.IsNullOrEmpty(model.TaxPayerCode) || model.RevenueHeadId <= 0)
                {
                    errors.Add(new ErrorModel { ErrorMessage = ErrorLang.invoice404(model.TaxPayerCode).ToString(), FieldName = "Invoice" });
                    throw new Exception("Model is empty");
                }

                StateConfig siteConfig = GetTenantConfig(headerParams.BILLERCODE);

                ShellContext shellContext = GetShellContext(siteConfig.FileSiteName);
                using (var workContext = shellContext.LifetimeScope.Resolve<IWorkContextAccessor>().CreateWorkContextScope())
                {
                    dynamic exheaderParams = new ExpandoObject();
                    exheaderParams.SIGNATURE = headerParams.SIGNATURE as string;
                    exheaderParams.CLIENTID = headerParams.CLIENTID as string;
                    return workContext.Resolve<IRDCAPICreateInvoiceHandler>().GenerateInvoice(model, exheaderParams);
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
        /// Get shell context for thsi operation
        /// </summary>
        /// <param name="siteName"></param>
        /// <returns>ShellContext</returns>
        public ShellContext GetShellContext(string siteName)
        {
            var tenantShellSettings = _shellSettingsManager.LoadSettings().Where(settings => settings.Name.Equals(siteName, StringComparison.OrdinalIgnoreCase)).Single();
            return _orchardHost.GetShellContext(tenantShellSettings);
        }


    }
}