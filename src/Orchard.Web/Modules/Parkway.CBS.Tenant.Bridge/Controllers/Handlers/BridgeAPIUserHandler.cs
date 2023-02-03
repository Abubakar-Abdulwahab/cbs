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
    public class BridgeAPIUserHandler : BaseAPIHandler, IBridgeAPIUserHandler
    {

        public ILogger Logger { get; set; }
        private readonly Lazy<IInvoicingService> _invoicingService;
        private readonly IShellSettingsManager _shellSettingsManager;
        private readonly IOrchardHost _orchardHost;

        public BridgeAPIUserHandler(Lazy<IInvoicingService> invoicingService, IShellSettingsManager shellSettingsManager, IOrchardHost orchardHost, IAdminSettingManager<ExpertSystemSettings> settingsRepository) : base(settingsRepository)
        {
            Logger = NullLogger.Instance;
            _invoicingService = invoicingService;
            _orchardHost = orchardHost;
            _shellSettingsManager = shellSettingsManager;
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
        /// Do model check
        /// </summary>
        /// <param name="callback">UserBridgeController</param>
        /// <returns>List{ErrorModel}</returns>
        public List<ErrorModel> DoModelCheck(UserBridgeController callback)
        {
            return CheckModelStateWithoutException(callback);
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

        public APIResponse SearchForTaxProfileByFilter(TaxProfilesSearchParams searchFilter, dynamic headerParams)
        {
            List<ErrorModel> errors = new List<ErrorModel>();
            string errorCode = ResponseCodeLang.generic_exception_code;

            try
            {
                if (string.IsNullOrEmpty(headerParams.BILLERCODE))
                {
                    errors.Add(new ErrorModel { ErrorMessage = ErrorLang.billercode("Biller code is required").ToString(), FieldName = "BillerCode" });
                    errorCode = ResponseCodeLang.record_404;
                    throw new Exception("Biller code is required");
                }
                StateConfig siteConfig = GetTenantConfig(headerParams.BILLERCODE);
                if (siteConfig == null)
                {
                    errors.Add(new ErrorModel { ErrorMessage = ErrorLang.billercode("Biller code not found").ToString(), FieldName = "BillerCode" });
                    errorCode = ResponseCodeLang.record_404;
                    throw new Exception("Biller code not found");
                }

                using (var workContext = GetShellContext(siteConfig.FileSiteName).LifetimeScope.Resolve<IWorkContextAccessor>().CreateWorkContextScope())
                {
                    dynamic exheaderParams = new ExpandoObject();
                    exheaderParams.SIGNATURE = headerParams.SIGNATURE as string;
                    exheaderParams.CLIENTID = headerParams.CLIENTID as string;
                    return workContext.Resolve<IAPIUserSettingsHandler>().SearchTaxProfilesByFilter(searchFilter, exheaderParams);
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                if(errors.Count < 1) { errors.Add(new ErrorModel { FieldName = "SearchModel", ErrorMessage = ErrorLang.genericexception().ToString() }); }
            }

            return new APIResponse
            {
                ResponseObject = errors,
                StatusCode = HttpStatusCode.BadRequest,
                Error = true,
                ErrorCode = errorCode
            };
        }
    }
}