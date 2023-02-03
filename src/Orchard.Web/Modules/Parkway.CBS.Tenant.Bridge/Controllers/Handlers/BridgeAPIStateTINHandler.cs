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
using Parkway.CBS.Module.API.Controllers.Handlers;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core;

namespace Parkway.CBS.Tenant.Bridge.Controllers.Handlers
{
    public class BridgeAPIStateTINHandler : BaseAPIHandler, IBridgeAPIStateTINHandler
    {

        public ILogger Logger { get; set; }
        private readonly Lazy<IInvoicingService> _invoicingService;
        private readonly IShellSettingsManager _shellSettingsManager;
        private readonly IOrchardHost _orchardHost;

        public BridgeAPIStateTINHandler(Lazy<IInvoicingService> invoicingService, IShellSettingsManager shellSettingsManager, IOrchardHost orchardHost, IAdminSettingManager<ExpertSystemSettings> settingsRepository) : base(settingsRepository)
        {
            Logger = NullLogger.Instance;
            _invoicingService = invoicingService;
            _orchardHost = orchardHost;
            _shellSettingsManager = shellSettingsManager;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="model"></param>
        /// <param name="headerParams"></param>
        /// <returns></returns>
        public APIResponse CreateStateTIN(StateTINBridgeController callback, CreateStateTINModel model, dynamic headerParams)
        {
            List<ErrorModel> errors = new List<ErrorModel>();
            HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest;
            string errorCode = ResponseCodeLang.generic_exception_code;

            try
            {
                if(model == null)
                {
                    errors.Add(new ErrorModel { ErrorMessage = ErrorLang.modelisempty().ToString(), FieldName = "StateTIN" });
                    throw new Exception("Model is empty");
                }

                CheckModelState(callback, ref errors);
                Logger.Information(string.Format("{0}", JsonConvert.SerializeObject(model)));

                if (string.IsNullOrEmpty(headerParams.BILLERCODE))
                {
                    errors.Add(new ErrorModel { ErrorMessage = ErrorLang.billercode("Biller code is required").ToString(), FieldName = "Biller Code" });
                    throw new Exception("Biller code is required");
                }
                StateConfig siteConfig = GetTenantConfig(headerParams.BILLERCODE);
                if (siteConfig == null)
                {
                    errors.Add(new ErrorModel { ErrorMessage = ErrorLang.billercode("Biller code not found").ToString(), FieldName = "Biller Code" });
                    throw new Exception("Biller code not found");
                }


                ShellContext shellContext = GetShellContext(siteConfig.FileSiteName);
                using (var workContext = shellContext.LifetimeScope.Resolve<IWorkContextAccessor>().CreateWorkContextScope())
                {
                    dynamic exheaderParams = new ExpandoObject();
                    exheaderParams.SIGNATURE = headerParams.SIGNATURE as string;
                    exheaderParams.CLIENTID = headerParams.CLIENTID as string;
                    return workContext.Resolve<IAPIStateTINHandler>().CreateStateTIN(model, exheaderParams);
                }

            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                errors.Add(new ErrorModel { FieldName = "StateTIN", ErrorMessage = ErrorLang.genericexception().ToString() });
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
        public ShellContext GetShellContext(string siteName)
        {
            var tenantShellSettings = _shellSettingsManager.LoadSettings().Where(settings => settings.Name.Equals(siteName, StringComparison.OrdinalIgnoreCase)).Single();
            return _orchardHost.GetShellContext(tenantShellSettings);
        }

        /// <summary>
        /// Do model check
        /// </summary>
        /// <param name="callback">StateTINBridgeController</param>
        /// <returns>List{ErrorModel}</returns>
        public List<ErrorModel> DoModelCheck(StateTINBridgeController callback)
        {
            return CheckModelStateWithoutException(callback);
        }

    }
}