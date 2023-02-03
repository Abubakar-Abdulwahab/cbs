using Autofac;
using Orchard;
using Orchard.Environment.ShellBuilders;
using Orchard.Logging;
using Orchard.Mvc.Filters;
using Orchard.UI.Admin;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.StateConfig;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.ReadyCash.Go.Controllers.Handlers.Contracts;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Parkway.CBS.ReadyCash.Go.Middleware.Filters
{
    public class RenderFormFieldsFilter : FilterProvider, IActionFilter
    {
        public ILogger Logger { get; set; }
        private readonly IRenderFormFieldsHandler _handler;

        public RenderFormFieldsFilter(IRenderFormFieldsHandler handler)
        {
            Logger = NullLogger.Instance;
            _handler = handler;
        }


        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var billerCode = filterContext.HttpContext.Request.Headers.Get("BILLERCODE");
            StateConfig siteConfig = _handler.GetTenantConfig(billerCode);

            Node node = siteConfig.Node.Where(x => x.Key == TenantConfigKeys.IsEnabledFormControlActionFilter.ToString()).FirstOrDefault();
            if (node != null && !string.IsNullOrEmpty(node.Value))
            {
                bool isActionFilterEnabled = false;
                bool.TryParse(node.Value, out isActionFilterEnabled);
                if (isActionFilterEnabled)
                {
                    try
                    {
                        //do work
                        var clientId = filterContext.HttpContext.Request.Headers.Get("CLIENTID");
                        var signatute = filterContext.HttpContext.Request.Headers.Get("SIGNATURE");
                        if (clientId == null || signatute == null || billerCode == null) { throw new Exception(); }

                        ShellContext shellContext = _handler.GetShellContext(siteConfig.FileSiteName);
                        using (var workContext = shellContext.LifetimeScope.Resolve<IWorkContextAccessor>().CreateWorkContextScope())
                        {
                            string clientSecret = workContext.Resolve<IAdminSettingManager<ExpertSystemSettings>>().GetClientSecretByClientId(clientId);
                            string valueString = $"{clientId}{filterContext.ActionParameters["revenueHead"]}";
                            if (Util.HMACHash256(valueString, clientSecret) != signatute) { throw new Exception(); }
                        }
                    }
                    catch (Exception exception)
                    {
                        Logger.Error(exception, string.Format("Exception in RenderFormFieldsFilterHandler. Exception Message --- {0}", exception.Message));
                        filterContext.Result = new HttpNotFoundResult();
                    }
                }
            }
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }
    }
}