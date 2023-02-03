using Orchard;
using Orchard.Logging;
using Orchard.UI.Admin;
using Orchard.UI.Notify;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Admin.RouteName;
using Parkway.CBS.Police.Core.Lang;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Parkway.CBS.Police.Admin.Controllers
{
    [Admin]
    public class DeploymentAllowancePaymentController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly INotifier _notifier;
        private readonly IDeploymentAllowancePaymentHandler _deploymentAllowancePaymentHandler;
        public ILogger Logger { get; set; }

        public DeploymentAllowancePaymentController(IOrchardServices orchardServices, IDeploymentAllowancePaymentHandler deploymentAllowancePaymentHandler)
        {
            _orchardServices = orchardServices;
            _deploymentAllowancePaymentHandler = deploymentAllowancePaymentHandler;
            _notifier = orchardServices.Notifier;
            Logger = NullLogger.Instance;
        }


        // GET: InitiatePaymentRequest
        public ActionResult InitiatePaymentRequest()
        {
            try
            {
                _deploymentAllowancePaymentHandler.CheckForPermission(Permissions.CanCreateDeploymentAllowancePaymentRequest);
                return View(_deploymentAllowancePaymentHandler.GetInitiateDeploymentAllowancePaymentVM());
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


        // POST: InitiatePaymentRequest
        [HttpPost]
        public ActionResult InitiatePaymentRequest(InitiateDeploymentAllowancePaymentVM userInputModel)
        {
            List<ErrorModel> errors = new List<ErrorModel> { };
            try
            {
                _deploymentAllowancePaymentHandler.CheckForPermission(Permissions.CanCreateDeploymentAllowancePaymentRequest);
                Logger.Information($"About to initiate deployment allowance payment request. Account wallet configuration id: {userInputModel.SelectedSourceAccountId}. User Id: {_orchardServices.WorkContext.CurrentUser.Id}");
                _notifier.Add(NotifyType.Information, PoliceLang.ToLocalizeString(_deploymentAllowancePaymentHandler.InitiateDeploymentAllowancePaymentRequest(userInputModel, ref errors)));
                return RedirectToRoute(DeploymentAllowancePaymentReport.Report);
            }
            catch (DirtyFormDataException)
            {
                Logger.Error(errors.First().ErrorMessage);
                _notifier.Add(NotifyType.Error, ErrorLang.ToLocalizeString(errors.First().ErrorMessage));
                _deploymentAllowancePaymentHandler.PopulateModelForPostback(userInputModel);
            }
            catch (CouldNotSaveRecord exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.genericexception());
                return Redirect("~/Admin");
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

            return View(userInputModel);
        }
    }
}