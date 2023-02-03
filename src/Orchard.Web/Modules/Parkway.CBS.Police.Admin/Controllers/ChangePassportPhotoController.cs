using Orchard;
using Orchard.Logging;
using Orchard.UI.Admin;
using Orchard.UI.Notify;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using System;
using System.Web.Mvc;
using Parkway.CBS.Police.Core.Lang;
using Parkway.CBS.Core.HelperModels;
using System.Collections.Generic;
using Parkway.CBS.Police.Core.HelperModels;

namespace Parkway.CBS.Police.Admin.Controllers
{
    [Admin]
    public class ChangePassportPhotoController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly INotifier _notifier;
        public ILogger Logger { get; set; }
        dynamic Shape { get; set; }

        private readonly IChangePassportHandler _changePassportPhotoHandler;
        private readonly IRequestListHandler _policeRequestHandler;


        public ChangePassportPhotoController(IOrchardServices orchardServices, IChangePassportHandler changePassportPhotoHandler, IRequestListHandler policeRequestHandler)
        {
            _orchardServices = orchardServices;
            _notifier = orchardServices.Notifier;
            Logger = NullLogger.Instance;
            _changePassportPhotoHandler = changePassportPhotoHandler;
            _policeRequestHandler = policeRequestHandler;
        }


        /// <summary>
        /// Get all requests
        /// </summary>
        /// <param name="userInput"></param>
        /// <param name="pagerParameters"></param>
        public ActionResult ChangePhoto()
        {
            try
            {
                _policeRequestHandler.CheckForPermission(Permissions.CanChangePassportPhotoForPCC);
                return View();
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
        /// Get escort or extract request view details
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="serviceTypeId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ChangePhoto(string passPortFileNumber)
        {
            List<ErrorModel> errors = new List<ErrorModel> { };

            try
            {
                _policeRequestHandler.CheckForPermission(Permissions.CanChangePassportPhotoForPCC);
                if (HttpContext.Request.Files.Get("passportPhotoFile") == null || HttpContext.Request.Files.Get("passportPhotoFile").ContentLength == 0)
                {
                    //errors.Add(new ErrorModel { FieldName = "SignatureFile", ErrorMessage = "No signature file specified." });
                    throw new DirtyFormDataException();
                }

                CharacterCertificateDocumentVM certDeets = _changePassportPhotoHandler.GetFileNumberDetails(passPortFileNumber).ResponseObject;
                _changePassportPhotoHandler.ValidatePassportPhoto(HttpContext.Request.Files.Get("passportPhotoFile"), ref errors);
                string filePathName = _changePassportPhotoHandler.SavePassportPhoto(HttpContext.Request.Files.Get("passportPhotoFile"), ref errors);

                if (errors.Count > 0) { throw new Exception { }; }

                bool saved = _changePassportPhotoHandler.ChangePassportPhoto(filePathName, certDeets);
                if (!saved)
                {
                    _notifier.Add(NotifyType.Error, Lang.custommessage("Error saving Passport photo for " + passPortFileNumber));
                }
                else
                {
                    _notifier.Add(NotifyType.Information, Lang.custommessage("Passport photo for " + passPortFileNumber + " saved succesfully"));
                }
                return RedirectToRoute(RouteName.ChangePassportPhoto.ChangePhoto);
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