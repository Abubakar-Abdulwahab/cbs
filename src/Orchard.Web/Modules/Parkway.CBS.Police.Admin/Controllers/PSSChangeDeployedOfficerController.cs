using Orchard;
using Orchard.Logging;
using Orchard.UI.Admin;
using Orchard.UI.Notify;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.Lang;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Parkway.CBS.Police.Admin.Controllers
{
    [Admin]
    public class PSSChangeDeployedOfficerController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly INotifier _notifier;
        private readonly IPSSChangeDeployedOfficerHandler _handler;
        public ILogger Logger { get; set; }

        public PSSChangeDeployedOfficerController(IOrchardServices orchardServices, IPSSChangeDeployedOfficerHandler handler)
        {
            _orchardServices = orchardServices;
            _notifier = orchardServices.Notifier;
            Logger = NullLogger.Instance;
            _handler = handler;
        }


        [HttpGet]
        public ActionResult ChangeDeployedOfficer(string deploymentId, string officerId)
        {
            try
            {
                _handler.CheckForPermission(Permissions.CanViewDeployments);
                if (String.IsNullOrEmpty(deploymentId)) { throw new Exception("Deployment Log Id not specified"); }
                if (String.IsNullOrEmpty(officerId)) { throw new Exception("Police Officer Id not specified"); }
                int deploymentLogId = 0;
                int policeOfficerId = 0;
                if (!int.TryParse(deploymentId, out deploymentLogId)) { throw new Exception($"Invalid Deployment Log Id {deploymentId}"); }
                if (!int.TryParse(officerId, out policeOfficerId)) { throw new Exception($"Invalid Police Officer Id {officerId}"); }
                ChangeDeployedOfficerVM vm = _handler.GetChangeDeployedOfficerVM(deploymentLogId);
                if (vm.CanNotBeChanged) { _notifier.Add(NotifyType.Warning, Lang.cannotchangepoliceofficer); }
                return View(vm);
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


        [HttpPost]
        public ActionResult ChangeDeployedOfficer(ChangeDeployedOfficerVM userInput)
        {
            string errorMessage = string.Empty;
            try
            {
                _handler.CheckForPermission(Permissions.CanViewDeployments);
                if (userInput.deploymentLogId <= 0) { throw new Exception($"Invalid Deployment Log Id {userInput.deploymentLogId}"); }
                if (userInput.selectedOfficer <= 0) { throw new Exception($"Invalid Police officer Id {userInput.selectedOfficer}"); }
                _handler.ChangeDeployedOfficer(userInput, ref errorMessage);
                _notifier.Add(NotifyType.Information, Lang.policeofficerchangedsuccessfully);
                return RedirectToRoute(Admin.RouteName.PSSDeployedOfficersReport.PSSDeployedOfficers);
            }
            catch(DirtyFormDataException)
            {
                _notifier.Add(NotifyType.Error, PoliceErrorLang.ToLocalizeString(errorMessage));
                return View(_handler.GetChangeDeployedOfficerVM(userInput.deploymentLogId));
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