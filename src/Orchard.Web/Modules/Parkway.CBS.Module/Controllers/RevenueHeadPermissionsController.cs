using Orchard;
using Orchard.Logging;
using Orchard.UI.Admin;
using Orchard.UI.Notify;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Module.Controllers.Handlers.Contracts;
using System;
using System.Web.Mvc;

namespace Parkway.CBS.Module.Controllers
{
    [Admin]
    public class RevenueHeadPermissionsController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly INotifier _notifier;
        private readonly IRevenueHeadPermissionsHandler _handler;
        public ILogger Logger { get; set; }
        private readonly Core.HTTP.Handlers.Contracts.ICoreSettingsService _se;

        public RevenueHeadPermissionsController(IOrchardServices orchardServices, IRevenueHeadPermissionsHandler handler, Core.HTTP.Handlers.Contracts.ICoreSettingsService se)
        {
            _orchardServices = orchardServices;
            _notifier = orchardServices.Notifier;
            _handler = handler;
            Logger = NullLogger.Instance;
            _se = se;
        }

        [HttpGet]
        public ActionResult Assign(string expertSystemId)
        {
            //try
            //{
            //    _se.DoExpertSystemAccessListMigration();
            //}
            //catch (Exception ex)
            //{
            //}
            try
            {
                int expertSystemIdParsed = 0;
                if(int.TryParse(expertSystemId, out expertSystemIdParsed))
                {
                    if (expertSystemIdParsed < 1) { _notifier.Add(NotifyType.Error, ErrorLang.genericexception()); return Redirect("~/Admin"); }
                    AssignRevenueHeadPermissionConstraintsVM model = _handler.GetRevenueHeadPermissionConstraintsVM(expertSystemIdParsed, 0);
                    return View(model);
                }
                else { _notifier.Add(NotifyType.Error, ErrorLang.genericexception()); return Redirect("~/Admin"); }
                
            }
            catch (UserNotAuthorizedForThisActionException)
            {
                var message = $"User ID {_orchardServices.WorkContext.CurrentUser.Id} tried to assign revenue head permissions without permission";
                Logger.Error(message);
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotauthorized());
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.genericexception());
            }
            return Redirect("~/Admin");
        }


        [HttpPost]
        public ActionResult Assign(AssignRevenueHeadPermissionConstraintsVM userInput)
        {
            var vm = new AssignRevenueHeadPermissionConstraintsVM { };
            try
            {
                if (!string.IsNullOrEmpty(userInput.SelectedPermissionId))
                {
                    _handler.AssignExpertSystemToSelectedRevenueHeads(userInput);
                    _notifier.Add(NotifyType.Information, Lang.savesuccessfully);
                    return RedirectToAction("Assign");
                }
                else
                {
                    vm = _handler.GetRevenueHeadPermissionConstraintsVM(userInput.ExpertSystem.Id, 0);
                    throw new Exception($"No valid mda or revenue head selected for assignment to expert system.");
                }
            }
            catch (UserNotAuthorizedForThisActionException)
            {
                var message = $"User ID {_orchardServices.WorkContext.CurrentUser.Id} tried to assign revenue head permissions without permission";
                Logger.Error(message);
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotauthorized());
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.genericexception());
                return Redirect("~/Admin");
            }

            userInput.MDAs = vm.MDAs;
            userInput.Permissions = vm.Permissions;
            userInput.ExpertSystem = vm.ExpertSystem;

            return View(userInput);
        }


        public JsonResult GetRevenueHeadsPerMda(string mdaIds)
        {
            try
            {
                var response = _handler.GetRevenueHeadsPerMda(mdaIds);
                return Json(new APIResponse { Error = false, ResponseObject = response });
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
            }

            return Json(new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().ToString() });
        }


        public JsonResult GetMdasForAccessType(string accessType)
        {
            try
            {
                var response = _handler.GetMDAsForAccessType(accessType);
                return Json(new APIResponse { Error = false, ResponseObject = response });
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
            }

            return Json(new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().ToString() });
        }
    }
}