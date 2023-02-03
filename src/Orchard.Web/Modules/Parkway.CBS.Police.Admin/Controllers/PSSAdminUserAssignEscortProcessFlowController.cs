using Orchard;
using Orchard.DisplayManagement;
using Orchard.Logging;
using Orchard.UI.Admin;
using Orchard.UI.Notify;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.Lang;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Parkway.CBS.Police.Admin.Controllers
{
    [Admin]
    public class PSSAdminUserAssignEscortProcessFlowController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly INotifier _notifier;
        private readonly IPSSAdminUserAssignEscortProcessFlowHandler _handler;
        ILogger Logger { get; set; }
        public PSSAdminUserAssignEscortProcessFlowController(INotifier notifier, IOrchardServices orchardServices, IPSSAdminUserAssignEscortProcessFlowHandler handler)
        {
            _handler = handler;
            Logger = NullLogger.Instance;
            _notifier = notifier;
            _orchardServices = orchardServices;
            Logger = NullLogger.Instance;
        }


        [HttpGet]
        public ActionResult AssignProcessFlow()
        {
            try
            {
                _handler.CheckForPermission(Permissions.CanAssignEscortProcessFlow);
                return View(_handler.GetAssignEscortProcessFlowVM());
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
        public ActionResult AssignProcessFlow(AssignEscortProcessFlowVM userInput)
        {
            try
            {
                _handler.CheckForPermission(Permissions.CanAssignEscortProcessFlow);
                List<ErrorModel> errors = new List<ErrorModel> { };
                _handler.AssignProcessFlowsToUsers(userInput, _orchardServices.WorkContext.CurrentUser.Id, ref errors);
                if(errors.Count > 0)
                {
                    userInput.CommandTypes = _handler.GetAssignEscortProcessFlowVM().CommandTypes;
                    _notifier.Add(NotifyType.Error, ErrorLang.ToLocalizeString(errors[0].ErrorMessage));
                    return View(userInput);
                }
                _notifier.Add(NotifyType.Information, PoliceLang.ToLocalizeString("Escort Process Flow(s) Assigned Successfully"));
                return RedirectToAction(nameof(RouteName.PSSAdminUserAssignEscortProcessFlow.AssignProcessFlow));
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