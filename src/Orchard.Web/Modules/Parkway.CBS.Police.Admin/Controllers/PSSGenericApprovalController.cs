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
using System.Linq;
using Parkway.CBS.Police.Admin.RouteName;
using Parkway.CBS.Police.Core.Lang;

namespace Parkway.CBS.Police.Admin.Controllers
{
    [Admin]
    public class PSSGenericApprovalController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly INotifier _notifier;
        public ILogger Logger { get; set; }

        private readonly IRequestApprovalHandler _requestApprovalHandler;
        private readonly IRequestListHandler _policeRequestHandler;


        public PSSGenericApprovalController(IOrchardServices orchardServices, IRequestApprovalHandler requestApprovalHandler, IRequestListHandler policeRequestHandler)
        {
            _orchardServices = orchardServices;
            _notifier = orchardServices.Notifier;
            Logger = NullLogger.Instance;
            _requestApprovalHandler = requestApprovalHandler;
            _policeRequestHandler = policeRequestHandler;
        }


        /// <summary>
        /// Approve pss generic request
        /// </summary>
        /// <param name="genericDetails"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ApproveRequest(GenericRequestDetailsVM genericDetails)
        {
            List<ErrorModel> errors = new List<ErrorModel> { };

            try
            {
                _policeRequestHandler.CheckForPermission(Permissions.CanApproveRequest);
                genericDetails.ApproverId = _orchardServices.WorkContext.CurrentUser.Id;
                RequestApprovalResponse details = _requestApprovalHandler.ProcessRequestApproval(genericDetails.RequestId, ref errors, genericDetails);                
                _notifier.Add(NotifyType.Information, Lang.pssrequestapprovedsuccessfully(details.NotificationMessage));

                return RedirectToRoute("pss.admin.request.list");
            }
            catch (DirtyFormDataException exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.ToLocalizeString(errors.First().ErrorMessage));
                return RedirectToRoute(RequestApproval.PSSRequestApproval, new { requestId = genericDetails.RequestId });
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
        /// Reject pss generic request
        /// </summary>
        /// <param name="extractDetails"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RejectRequest(GenericRequestDetailsVM genericDetails)
        {
            List<ErrorModel> errors = new List<ErrorModel> { };

            try
            {
                _policeRequestHandler.CheckForPermission(Permissions.CanViewRequests);
                RequestApprovalResponse details = _requestApprovalHandler.ProcessRequestRejection(genericDetails.RequestId, ref errors, genericDetails.Comment, _orchardServices.WorkContext.CurrentUser.Id);
                _notifier.Add(NotifyType.Information, Lang.pssrequestrejectedsuccessfully(details.NotificationMessage));
                return RedirectToRoute("pss.admin.request.list");
            }
            catch (DirtyFormDataException exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.ToLocalizeString(errors.First().ErrorMessage));
                return RedirectToRoute(RequestApproval.PSSRequestApproval, new { requestId = genericDetails.RequestId });
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