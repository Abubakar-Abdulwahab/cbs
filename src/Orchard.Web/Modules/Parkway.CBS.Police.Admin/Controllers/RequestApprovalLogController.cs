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
using System.Linq;
using System.Web.Mvc;

namespace Parkway.CBS.Police.Admin.Controllers
{
    [Admin]
    public class RequestApprovalLogController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly INotifier _notifier;
        public ILogger Logger { get; set; }
        private readonly IRequestApprovalLogHandler _handler;
        private readonly IRequestListHandler _policeRequestHandler;

        public RequestApprovalLogController(IOrchardServices orchardServices, INotifier notifier, IRequestApprovalLogHandler handler, IRequestListHandler policeRequestHandler)
        {
            _orchardServices = orchardServices;
            _notifier = notifier;
            Logger = NullLogger.Instance;
            _handler = handler;
            _policeRequestHandler = policeRequestHandler;
        }


        // GET: RequestApprovalLog
        public ActionResult RequestApprovalLog(Int64 requestId)
        {
            try
            {
                _policeRequestHandler.CheckForPermission(Permissions.CanViewRequests);
                if (requestId > 0)
                {
                        RequestApprovalLogVM vm = _handler.GetRequestApprovalLogVMByRequestId(requestId);
                        if(vm.ApprovalLog == null || !vm.ApprovalLog.Any())
                        {
                            _notifier.Add(NotifyType.Information, PoliceLang.requesthasnotyetbeenapproved);
                        }
                        return View(vm);
                }
                else { throw new Exception("Request Id not specified"); }

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