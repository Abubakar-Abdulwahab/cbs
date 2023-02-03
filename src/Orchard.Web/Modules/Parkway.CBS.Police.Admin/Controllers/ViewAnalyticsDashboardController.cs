using Orchard;
using Orchard.Logging;
using Orchard.UI.Admin;
using Orchard.UI.Notify;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.StateConfig;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.Lang;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.Utilities;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Parkway.CBS.Police.Admin.Controllers
{
    [Admin]
    public class ViewAnalyticsDashboardController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly INotifier _notifier;
        public ILogger Logger { get; set; }
        private readonly IRequestListHandler _policeRequestHandler;

        public ViewAnalyticsDashboardController(IOrchardServices orchardServices, IRequestListHandler policeRequestHandler)
        {
            _orchardServices = orchardServices;
            _notifier = orchardServices.Notifier;
            Logger = NullLogger.Instance;
            _policeRequestHandler = policeRequestHandler;
        }

        public ActionResult ViewAnalytics()
        {
            try
            {
                _policeRequestHandler.CheckForPermission(Permissions.CanViewAnalyticsDashboard);
                var stateConfig = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName);
                if (stateConfig == null)
                {
                    throw new Exception(PoliceErrorLang.ToLocalizeString("Unable to get State Config configuration").Text);
                }
                Node encryptionKeyNode = stateConfig.Node.Where(x => x.Key == nameof(PSSTenantConfigKeys.POSSAPAnalyticsEncryptionKey)).FirstOrDefault();
                Node analyticsURLNode = stateConfig.Node.Where(x => x.Key == nameof(PSSTenantConfigKeys.AnalyticsDashboardURL)).FirstOrDefault();
                if (encryptionKeyNode == null || string.IsNullOrEmpty(encryptionKeyNode.Value))
                {
                    throw new Exception(PoliceErrorLang.ToLocalizeString("Analyics encryption Key not found").Text);
                }
                if (analyticsURLNode == null || string.IsNullOrEmpty(analyticsURLNode.Value))
                {
                    throw new Exception(PoliceErrorLang.ToLocalizeString("Analyics dashboard URL not found").Text);
                }

                return Redirect($"{analyticsURLNode.Value}?typ={PSSUtil.AnalyticsDashboardEncrypt(_orchardServices.WorkContext.CurrentUser.Id.ToString(), encryptionKeyNode.Value)}");
            }
            catch (UserNotAuthorizedForThisActionException)
            {
                _notifier.Add(NotifyType.Warning, PoliceErrorLang.usernotauthorized());
                return Redirect("~/Admin");
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.genericexception());
                return Redirect("~/Admin");
            }
        }
    }
}