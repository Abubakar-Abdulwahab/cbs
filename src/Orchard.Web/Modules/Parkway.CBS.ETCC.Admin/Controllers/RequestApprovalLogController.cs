using Orchard;
using Orchard.Logging;
using Orchard.UI.Admin;
using Orchard.UI.Notify;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.ETCC.Admin.Controllers.Handlers.Contracts;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Parkway.CBS.ETCC.Admin.Controllers
{
    [Admin]
    public class RequestApprovalLogController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly INotifier _notifier;
        public ILogger Logger { get; set; }
        private readonly ITCCReportHandler _tccReportHandler;
        private readonly IRequestApprovalLogHandler _requestApprovalLogHandler;

        public RequestApprovalLogController(IOrchardServices orchardServices, INotifier notifier, ITCCReportHandler tccReportHandler, IRequestApprovalLogHandler requestApprovalLogHandler)
        {
            _orchardServices = orchardServices;
            _notifier = notifier;
            Logger = NullLogger.Instance;
            _tccReportHandler = tccReportHandler;
            _requestApprovalLogHandler = requestApprovalLogHandler;
        }


        // GET: RequestApprovalLog
        public ActionResult PreviewApprovalLog(string applicationNumber)
        {
            try
            {
                _tccReportHandler.CheckForPermission(Permissions.CanViewTCCRequests);
                if (!string.IsNullOrEmpty(applicationNumber))
                {
                    TCCRequestApprovalLogVM vm = _requestApprovalLogHandler.GetRequestApprovalLogVM(applicationNumber);
                    if (vm.ApprovalLog == null || !vm.ApprovalLog.Any())
                    {
                        _notifier.Add(NotifyType.Information, ErrorLang.requesthasnotyetbeenapproved);
                    }
                    return View(vm);
                }
                else { throw new Exception("Appli Id not specified"); }

            }
            catch (NoRecordFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Warning, ErrorLang.norecord404(exception.Message));
                return Redirect("~/Admin");
            }
            catch (UserNotAuthorizedForThisActionException)
            {
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotauthorized());
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