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
using System.Collections.Generic;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Admin.RouteName;
using System.Linq;
using Parkway.CBS.Police.Core.Lang;

namespace Parkway.CBS.Police.Admin.Controllers
{
    [Admin]
    public class PSSCharacterCertificateApprovalController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly INotifier _notifier;
        public ILogger Logger { get; set; }

        private readonly IRequestApprovalHandler _requestApprovalHandler;
        private readonly IRequestListHandler _policeRequestHandler;
        private readonly IPSSCharacterCertificateRequestDetailsHandler _certificateRequestDetailsHandler;

        public PSSCharacterCertificateApprovalController(IOrchardServices orchardServices, IRequestApprovalHandler requestApprovalHandler, IRequestListHandler policeRequestHandler, IPSSCharacterCertificateRequestDetailsHandler certificateRequestDetailsHandler)
        {
            _orchardServices = orchardServices;
            _notifier = orchardServices.Notifier;
            Logger = NullLogger.Instance;
            _requestApprovalHandler = requestApprovalHandler;
            _policeRequestHandler = policeRequestHandler;
            _certificateRequestDetailsHandler = certificateRequestDetailsHandler;
        }


        /// <summary>
        /// Approve extract request
        /// </summary>
        /// <param name="extractDetails"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ApproveRequest(CharacterCertificateRequestDetailsVM characterCertificateDetails)
        {
            List<ErrorModel> errors = new List<ErrorModel> { };
            try
            {
                _policeRequestHandler.CheckForPermission(Permissions.CanApproveRequest);
                characterCertificateDetails.ApproverId = _orchardServices.WorkContext.CurrentUser.Id;
                RequestApprovalResponse details = _requestApprovalHandler.ProcessRequestApproval(characterCertificateDetails.RequestId, ref errors, characterCertificateDetails);
                _notifier.Add(NotifyType.Information, Lang.pssrequestapprovedsuccessfully(details.NotificationMessage));
                return RedirectToRoute("pss.admin.request.list");
            }
            catch (DirtyFormDataException exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.ToLocalizeString(errors.First().ErrorMessage));
                return RedirectToRoute(RequestApproval.PSSRequestApproval, new { requestId = characterCertificateDetails.RequestId });
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
        /// Reject character certificate request
        /// </summary>
        /// <param name="characterCertificateDetails"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RejectRequest(CharacterCertificateRequestDetailsVM characterCertificateDetails)
        {
            List<ErrorModel> errors = new List<ErrorModel> { };
            try
            {
                _policeRequestHandler.CheckForPermission(Permissions.CanApproveRequest);
                characterCertificateDetails.ApproverId = _orchardServices.WorkContext.CurrentUser.Id;
                RequestApprovalResponse details = _requestApprovalHandler.ProcessRequestRejection(characterCertificateDetails.RequestId, ref errors, characterCertificateDetails.Comment, _orchardServices.WorkContext.CurrentUser.Id);
                characterCertificateDetails.FileRefNumber = details.FileNumber;
                _certificateRequestDetailsHandler.CreateAndSaveRejectionCertificate(characterCertificateDetails, ref errors);

                _notifier.Add(NotifyType.Information, Lang.pssrequestrejectedsuccessfully(details.NotificationMessage));
                return RedirectToRoute("pss.admin.request.list");
            }
            catch (DirtyFormDataException exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.ToLocalizeString(errors.First().ErrorMessage));
                return RedirectToRoute(RequestApproval.PSSRequestApproval, new { requestId = characterCertificateDetails.RequestId });
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
        /// Invite applicant for biometric capture
        /// </summary>
        /// <param name="characterCertificateDetails"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult InviteApplicant(CharacterCertificateRequestDetailsVM characterCertificateDetails)
        {
            List<ErrorModel> errors = new List<ErrorModel> { };
            try
            {
                _policeRequestHandler.CheckForPermission(Permissions.CanApproveRequest);
                characterCertificateDetails.ApproverId = _orchardServices.WorkContext.CurrentUser.Id;
                string details = _certificateRequestDetailsHandler.InviteApplicantForBiometricCapture(characterCertificateDetails.RequestId);
                _notifier.Add(NotifyType.Information, PoliceLang.applicantinvitedsuccessfully(details));
                return RedirectToRoute("pss.admin.request.list");
            }
            catch (DirtyFormDataException exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.ToLocalizeString(errors.First().ErrorMessage));
                return RedirectToRoute(RequestApproval.PSSRequestApproval, new { requestId = characterCertificateDetails.RequestId });
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