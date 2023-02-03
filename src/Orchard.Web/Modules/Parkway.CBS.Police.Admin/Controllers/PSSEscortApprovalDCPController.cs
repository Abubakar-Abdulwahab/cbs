using Orchard;
using Orchard.Logging;
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
    public class PSSEscortApprovalDCPController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly INotifier _notifier;
        public ILogger Logger { get; set; }

        private readonly IRequestListHandler _policeRequestHandler;
        private readonly IPSSEscortApprovalHandler _handler;

        public PSSEscortApprovalDCPController(IOrchardServices orchardServices, IRequestListHandler policeRequestHandler, IPSSEscortApprovalHandler handler)
        {
            _orchardServices = orchardServices;
            _notifier = orchardServices.Notifier;
            Logger = NullLogger.Instance;
            _policeRequestHandler = policeRequestHandler;
            _handler = handler;
        }


        /// <summary>
        /// DCP Approve escort request
        /// </summary>
        /// <param name="requestDetailsVM"></param>
        [HttpPost]
        public ActionResult ApproveRequest(EscortRequestDetailsVM requestDetailsVM)
        {
            try
            {
                _policeRequestHandler.CheckForPermission(Permissions.CanApproveRequest);
                requestDetailsVM.ApproverId = _orchardServices.WorkContext.CurrentUser.Id;

                EscortApprovalMessage response = _handler.ProcessCustomApproval(requestDetailsVM.RequestId, requestDetailsVM.ApproverId);
                if (!string.IsNullOrEmpty(response.Message) && !response.CanApproveRequest)
                {
                    _notifier.Add(NotifyType.Information, Lang.pssrequestapprovedsuccessfully(response.Message));
                    return RedirectToRoute(RouteName.RequestApproval.PSSRequestApproval, new { requestId = requestDetailsVM.RequestId });
                }
                _notifier.Add(NotifyType.Information, Lang.pssrequestapprovedsuccessfully(response.Message));
                return RedirectToRoute("pss.admin.request.list");
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