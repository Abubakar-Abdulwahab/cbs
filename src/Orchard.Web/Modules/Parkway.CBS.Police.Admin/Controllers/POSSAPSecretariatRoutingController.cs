using Orchard;
using Orchard.Logging;
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
using System.Web;
using System.Web.Mvc;

namespace Parkway.CBS.Police.Admin.Controllers
{
    public class POSSAPSecretariatRoutingController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly INotifier _notifier;
        private readonly IPOSSAPSecretariatRoutingHandler _iPOSSAPSecretariatRoutingHandler;
        public ILogger Logger { get; set; }

        public POSSAPSecretariatRoutingController(IOrchardServices orchardServices, IPOSSAPSecretariatRoutingHandler iPOSSAPSecretariatRoutingHandler)
        {
            _orchardServices = orchardServices;
            _notifier = orchardServices.Notifier;
            _iPOSSAPSecretariatRoutingHandler = iPOSSAPSecretariatRoutingHandler;
            Logger = NullLogger.Instance;
        }


        [HttpPost]
        public ActionResult RouteForEscort(EscortRequestDetailsVM requestDetailsVM)
        {
            List<ErrorModel> errors = new List<ErrorModel> { };
            try
            {
                _iPOSSAPSecretariatRoutingHandler.CheckForPermission(Permissions.CanApproveRequest);
                requestDetailsVM.ApproverId = _orchardServices.WorkContext.CurrentUser.Id;
                var response = _iPOSSAPSecretariatRoutingHandler.RouteToEscortStage(requestDetailsVM, ref errors);
                _notifier.Add(NotifyType.Information, ErrorLang.ToLocalizeString(response.NotificationMessage));
                return RedirectToRoute(RequestApproval.PSSRequestList);
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


        [HttpPost]
        public ActionResult RouteForCharacterCertificate(CharacterCertificateRequestDetailsVM requestDetailsVM)
        {
            List<ErrorModel> errors = new List<ErrorModel> { };
            try
            {
                _iPOSSAPSecretariatRoutingHandler.CheckForPermission(Permissions.CanApproveRequest);
                requestDetailsVM.ApproverId = _orchardServices.WorkContext.CurrentUser.Id;
                var response = _iPOSSAPSecretariatRoutingHandler.RouteToCharacterCertificateStage(requestDetailsVM, ref errors);
                _notifier.Add(NotifyType.Information, ErrorLang.ToLocalizeString(response.NotificationMessage));
                return RedirectToRoute(RequestApproval.PSSRequestList);
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
    }
}