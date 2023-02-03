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
using System.Web.Mvc;

namespace Parkway.CBS.Police.Admin.Controllers
{
    [Admin]
    public class PSSEndOfficerDeploymentController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly INotifier _notifier;
        private readonly IPSSEndOfficerDeploymentHandler _handler;
        public ILogger Logger { get; set; }

        public PSSEndOfficerDeploymentController(IOrchardServices orchardServices, IPSSEndOfficerDeploymentHandler handler)
        {
            _orchardServices = orchardServices;
            _notifier = orchardServices.Notifier;
            Logger = NullLogger.Instance;
            _handler = handler;
        }


        [HttpGet]
        public ActionResult EndOfficerDeployment(string deploymentId)
        {
            try
            {
                _handler.CheckForPermission(Permissions.CanEndOfficerDeployment);
                if (String.IsNullOrEmpty(deploymentId)) { throw new Exception("Deployment Log Id not specified"); }
                int deploymentLogId = 0;
                if (!int.TryParse(deploymentId, out deploymentLogId)) { throw new Exception($"Invalid Deployment Log Id {deploymentId}"); }
                EndOfficerDeploymentVM vm = _handler.GetDeployedOfficerDetails(deploymentLogId);
                if (vm.CanNotEndDeployment) { _notifier.Add(NotifyType.Warning, PoliceLang.cannotendofficerdeployment); }
                return View(vm);
            }
            catch (UserNotAuthorizedForThisActionException)
            {
                _notifier.Add(NotifyType.Warning, PoliceErrorLang.usernotauthorized());
                return Redirect("~/Admin");
            }
            catch (NoRecordFoundException ex)
            {
                Logger.Error(ex, ex.Message);
                _notifier.Add(NotifyType.Warning, ErrorLang.norecord404());
                return RedirectToRoute(RouteName.PSSDeployedOfficersReport.PSSDeployedOfficers);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.genericexception());
                return Redirect("~/Admin");
            }
        }


        [HttpPost]
        public ActionResult EndOfficerDeployment(EndOfficerDeploymentVM userInput)
        {
            try
            {
                _handler.CheckForPermission(Permissions.CanEndOfficerDeployment);
                if (userInput.DeploymentLogId <= 0) { throw new Exception($"Invalid Deployment Log Id {userInput.DeploymentLogId}"); }
                _handler.EndOfficerDeployment(userInput, _orchardServices.WorkContext.CurrentUser.Id);
                _notifier.Add(NotifyType.Information, PoliceLang.officerdeploymentendedsuccessfully);
                return RedirectToRoute(RouteName.PSSDeployedOfficersReport.PSSDeployedOfficers);
            }
            catch (UserNotAuthorizedForThisActionException)
            {
                _notifier.Add(NotifyType.Warning, PoliceErrorLang.usernotauthorized());
                return Redirect("~/Admin");
            }
            catch (NoRecordFoundException ex)
            {
                Logger.Error(ex, ex.Message);
                _notifier.Add(NotifyType.Warning, ErrorLang.norecord404());
                return RedirectToRoute(RouteName.PSSDeployedOfficersReport.PSSDeployedOfficers);
            }
            catch (DirtyFormDataException exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.ToLocalizeString(exception.Message));
                return RedirectToRoute(RouteName.PSSDeployedOfficersReport.PSSDeployedOfficers);
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