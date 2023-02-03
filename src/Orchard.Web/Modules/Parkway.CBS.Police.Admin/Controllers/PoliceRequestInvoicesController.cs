using Orchard;
using Orchard.Logging;
using Orchard.UI.Admin;
using Orchard.UI.Notify;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.Lang;
using System;
using System.Web.Mvc;

namespace Parkway.CBS.Police.Admin.Controllers
{
    [Admin]
    public class PoliceRequestInvoicesController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly INotifier _notifier;
        public ILogger Logger { get; set; }
        private readonly IRequestListHandler _policeRequestHandler;

        public PoliceRequestInvoicesController(IOrchardServices orchardServices, IRequestListHandler policeRequestHandler)
        {
            _orchardServices = orchardServices;
            _notifier = orchardServices.Notifier;
            Logger = NullLogger.Instance;
            _policeRequestHandler = policeRequestHandler;
        }


        /// <summary>
        /// Get invoices for request with specified Id
        /// </summary>
        /// <param name="fileNumber"></param>
        /// <returns></returns>
        public ActionResult PSSRequestsInvoices(long requestId)
        {
            try
            {
                _policeRequestHandler.CheckForPermission(Permissions.CanViewRequests);

                if (requestId > 0)
                {
                    var invoices = _policeRequestHandler.GetInvoicesForRequest(requestId);
                    return View(invoices);
                }
                else { throw new Exception("Invalid request Id"); }

            }
            catch (NoRecordFoundException)
            {
                _notifier.Add(NotifyType.Warning, ErrorLang.record404());
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
        }
    }
}