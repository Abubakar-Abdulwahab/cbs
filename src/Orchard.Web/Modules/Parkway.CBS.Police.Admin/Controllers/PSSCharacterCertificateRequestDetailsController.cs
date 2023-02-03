using Orchard;
using Orchard.Logging;
using Orchard.UI.Admin;
using Orchard.UI.Notify;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Lang;
using System;
using System.Web.Mvc;

namespace Parkway.CBS.Police.Admin.Controllers
{

    public class PSSCharacterCertificateRequestDetailsController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly INotifier _notifier;
        private readonly IPSSCharacterCertificateRequestDetailsHandler _handler;
        ILogger Logger { get; set; }


        public PSSCharacterCertificateRequestDetailsController(IOrchardServices orchardServices, IPSSCharacterCertificateRequestDetailsHandler handler)
        {
            _orchardServices = orchardServices;
            _notifier = orchardServices.Notifier;
            _handler = handler;
            Logger = NullLogger.Instance;
        }

        /// <summary>
        /// View certificate
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <returns></returns>
        public ActionResult ViewCertificate(string fileRefNumber)
        {
            try
            {
                CreateCertificateDocumentVM result = _handler.CreateCharacterCertificateByteFile(fileRefNumber);
                System.Web.HttpContext.Current.Response.ClearHeaders();
                System.Web.HttpContext.Current.Response.ContentType = "application/pdf";
                System.Web.HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + result.FileName);
                System.Web.HttpContext.Current.Response.TransmitFile(result.SavedPath);
                System.Web.HttpContext.Current.Response.End();
                return null;
            }
            catch (UserNotAuthorizedForThisActionException exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, PoliceErrorLang.usernotauthorized());
            }
            catch (NoRecordFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.norecord404());
            }
            catch (Exception ex)
            {
                _notifier.Error(ErrorLang.genericexception());
                Logger.Error(ex, ex.Message);
            }
            return Redirect("~/Admin");
        }

        /// <summary>
        /// Gets view model for Character Certificate Biometrics by <paramref name="requestId"/>
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        [Admin]
        public ActionResult ViewFingerPrintBiometrics(long requestId)
        {
            try
            {
                _handler.CheckForPermission(Permissions.CanViewRequests);

                Core.VM.CharacterCertificateBiometricsVM model = _handler.GetCharacterCertificateBiometrics(requestId);
                return View(model);
            }
            catch (UserNotAuthorizedForThisActionException exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, PoliceErrorLang.usernotauthorized());
            }
            catch (NoRecordFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.norecord404());
            }
            catch (Exception ex)
            {
                _notifier.Error(ErrorLang.genericexception());
                Logger.Error(ex, ex.Message);
            }
            return Redirect("~/Admin");
        }
    }
}