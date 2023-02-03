using Autofac;
using Orchard;
using Orchard.Environment;
using Orchard.Environment.Configuration;
using Orchard.Environment.ShellBuilders;
using Orchard.Logging;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.StateConfig;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.ReadyCash.Go.Controllers.Handlers.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.ReadyCash.Go.Controllers.Handlers
{
    public class RenderFormFieldsHandler : IRenderFormFieldsHandler
    {
        public ILogger Logger { get; set; }
        private readonly IShellSettingsManager _shellSettingsManager;
        private readonly IOrchardHost _orchardHost;



        public RenderFormFieldsHandler(IShellSettingsManager shellSettingsManager, IOrchardHost orchardHost)
        {
            Logger = NullLogger.Instance;
            _orchardHost = orchardHost;
            _shellSettingsManager = shellSettingsManager;
        }


        /// <summary>
        /// Get RevenueHead form controls using the revenueheadid, category and tenant sitename
        /// </summary>
        /// <param name="revenueHeadId"></param>
        /// <param name="payerId"></param>
        /// <param name="fileSiteName"></param>
        /// <returns>IEnumerable<FormControlViewModel></returns>
        public IEnumerable<FormControlViewModel> GetFormControlsForRevenueHead(int revenueHeadId, string payerId, string fileSiteName)
        {
            try
            {
                ShellContext shellContext = GetShellContext(fileSiteName);
                using (var workContext = shellContext.LifetimeScope.Resolve<IWorkContextAccessor>().CreateWorkContextScope())
                {
                    return workContext.Resolve<ICoreFormService>().GetRevenueHeadFormFields(revenueHeadId, payerId).ToList();
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Get tenant config settings
        /// </summary>
        /// <param name="vendorCode"></param>
        /// <returns>StateConfig</returns>
        public StateConfig GetTenantConfig(string vendorCode)
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