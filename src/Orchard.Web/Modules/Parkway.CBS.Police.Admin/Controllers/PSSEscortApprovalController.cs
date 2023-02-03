using Orchard;
using Orchard.Logging;
using Orchard.UI.Admin;
using Orchard.UI.Notify;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using System;
using System.Web.Mvc;
using Parkway.CBS.Police.Core.VM;
using Parkway.CBS.Core.HelperModels;
using System.Collections.Generic;
using Parkway.CBS.Police.Admin.RouteName;
using System.Linq;
using Parkway.CBS.Police.Core.Lang;

namespace Parkway.CBS.Police.Admin.Controllers
{
    [Admin]
    public class PSSEscortApprovalController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly INotifier _notifier;
        public ILogger Logger { get; set; }

        private readonly IRequestApprovalHandler _requestApprovalHandler;
        private readonly IRequestListHandler _policeRequestHandler;
        private readonly IPSSEscortApprovalHandler _handler;

        public PSSEscortApprovalController(IOrchardServices orchardServices, IRequestApprovalHandler requestApprovalHandler, IRequestListHandler policeRequestHandler, IPSSEscortApprovalHandler handler)
        {
            _orchardServices = orchardServices;
            _notifier = orchardServices.Notifier;
            Logger = NullLogger.Instance;
            _requestApprovalHandler = requestApprovalHandler;
            _policeRequestHandler = policeRequestHandler;
            _handler = handler;
        }


        /// <summary>
        /// Approve escort request
        /// </summary>
        /// <param name="requestDetailsVM"></param>
        [HttpPost]
        public ActionResult ApproveRequest(EscortRequestDetailsVM requestDetailsVM)
        {
            List<ErrorModel> errors = new List<ErrorModel> { };
            try
            {
                _policeRequestHandler.CheckForPermission(Permissions.CanApproveRequest);
                requestDetailsVM.ApproverId = _orchardServices.WorkContext.CurrentUser.Id;

                RequestApprovalResponse details = _requestApprovalHandler.ProcessRequestApproval(requestDetailsVM.RequestId, ref errors, requestDetailsVM);
                _notifier.Add(NotifyType.Information, Lang.pssrequestapprovedsuccessfully(details.NotificationMessage));
                return RedirectToRoute("pss.admin.request.list");
            }
            catch (DirtyFormDataException exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.ToLocalizeString(errors.First().ErrorMessage));
                return RedirectToRoute(RequestApproval.PSSRequestApproval, new { requestId = requestDetailsVM.RequestId });
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



        /// <summary>
        /// Reject escort request
        /// </summary>
        /// <param name="requestDetailsVM"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RejectRequest(EscortRequestDetailsVM requestDetailsVM)
        {
            List<ErrorModel> errors = new List<ErrorModel> { };

            try
            {
                _policeRequestHandler.CheckForPermission(Permissions.CanViewRequests);
                RequestApprovalResponse details = _requestApprovalHandler.ProcessRequestRejection(requestDetailsVM.RequestId, ref errors, requestDetailsVM.Comment, _orchardServices.WorkContext.CurrentUser.Id);
                _notifier.Add(NotifyType.Information, Lang.pssrequestrejectedsuccessfully(details.NotificationMessage));
                return RedirectToRoute("pss.admin.request.list");
            }
            catch (DirtyFormDataException exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.ToLocalizeString(errors.First().ErrorMessage));
                return RedirectToRoute(RequestApproval.PSSRequestApproval, new { requestId = requestDetailsVM.RequestId });
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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rankId"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual JsonResult GetRankDetails(int rankId)
        {
            APIResponse response = _requestApprovalHandler.GetPoliceRank(rankId);
            return Json(response, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// View Allocated Formations
        /// </summary>
        /// <param name="squadAllocationId"></param>
        /// <param name="squadAllocationGroupId"></param>
        /// <returns></returns>
        public ActionResult ViewFormations(long squadAllocationId, long squadAllocationGroupId)
        {
            try
            {
                _policeRequestHandler.CheckForPermission(Permissions.CanViewRequests);
                if (squadAllocationId == 0 || squadAllocationGroupId == 0) { return View(new List<AIGFormationVM> { }); }
                return View(_handler.GetFormationsAllocatedToSquad(squadAllocationId, squadAllocationGroupId));
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