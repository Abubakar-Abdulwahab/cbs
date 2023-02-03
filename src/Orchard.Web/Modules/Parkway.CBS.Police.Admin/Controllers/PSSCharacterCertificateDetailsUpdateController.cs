using Orchard;
using Orchard.Logging;
using Orchard.UI.Admin;
using Orchard.UI.Notify;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Admin.RouteName;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Lang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Parkway.CBS.Police.Admin.Controllers
{
    [Admin]
    public class PSSCharacterCertificateDetailsUpdateController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly INotifier _notifier;
        public ILogger Logger { get; set; }
        private readonly IRequestListHandler _policeRequestHandler;
        private readonly IPSSCharacterCertificateUpdateDetailsHandler _charcaterCertificateUpdateDetailsHandler;

        public PSSCharacterCertificateDetailsUpdateController(IOrchardServices orchardServices, IRequestListHandler policeRequestHandler, IPSSCharacterCertificateUpdateDetailsHandler charcaterCertificateUpdateDetailsHandler)
        {
            _orchardServices = orchardServices;
            _notifier = orchardServices.Notifier;
            Logger = NullLogger.Instance;
            _policeRequestHandler = policeRequestHandler;
            _charcaterCertificateUpdateDetailsHandler = charcaterCertificateUpdateDetailsHandler;
        }

        [HttpGet]
        public ActionResult SearchFileNumber()
        {
            try
            {
                _policeRequestHandler.CheckForPermission(Permissions.CanUpdateDetailsForPCC);
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


        [HttpPost]
        public ActionResult SearchFileNumber(Core.VM.SearchByFileNumberVM model)
        {
            try
            {
                _policeRequestHandler.CheckForPermission(Permissions.CanUpdateDetailsForPCC);

                if (string.IsNullOrEmpty(model.FileNumber?.Trim()))
                {
                    _notifier.Add(NotifyType.Error, Lang.custommessage("Please enter a valid file number."));
                    return RedirectToRoute(PSSCharacterCertificateDetailsUpdate.SearchFileNumber);
                }
                return RedirectToRoute(PSSCharacterCertificateDetailsUpdate.UpdateDetails, new { fileNumber = model.FileNumber });
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

        [HttpGet]
        public ActionResult UpdateDetails(string fileNumber)
        {
            try
            {
                _policeRequestHandler.CheckForPermission(Permissions.CanUpdateDetailsForPCC);
                CharacterCertificateDetailsUpdateVM result = _charcaterCertificateUpdateDetailsHandler.GetFileNumberDetails(fileNumber.Trim());
                result.Countries = _charcaterCertificateUpdateDetailsHandler.GetCountries();
                return View(result);
            }
            catch (UserNotAuthorizedForThisActionException)
            {
                _notifier.Add(NotifyType.Warning, PoliceErrorLang.usernotauthorized());
                return Redirect("~/Admin");
            }
            catch (NoRecordFoundException)
            {
                _notifier.Add(NotifyType.Warning, ErrorLang.norecord404());
                return RedirectToRoute(PSSCharacterCertificateDetailsUpdate.SearchFileNumber);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.genericexception());
                return Redirect("~/Admin");
            }
        }


        [HttpPost]
        public ActionResult UpdateDetails(CharacterCertificateDetailsUpdateVM model)
        {
            List<ErrorModel> errors = new List<ErrorModel> { };
            try
            {
                _policeRequestHandler.CheckForPermission(Permissions.CanUpdateDetailsForPCC);
                bool saved = _charcaterCertificateUpdateDetailsHandler.UpdateCharacterCertificateDetails(model, out errors);
                if (!saved)
                {
                    _notifier.Add(NotifyType.Error, Lang.custommessage("Error saving character certificate details for file number " + model.FileNumber));
                    return RedirectToRoute(PSSCharacterCertificateDetailsUpdate.UpdateDetails, new { fileNumber = model.FileNumber });
                }

                _notifier.Add(NotifyType.Information, Lang.custommessage("Details for File Number " + model.FileNumber + " updated succesfully"));
                return RedirectToRoute(PSSCharacterCertificateDetailsUpdate.SearchFileNumber);
            }
            catch (DirtyFormDataException exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.ToLocalizeString(errors.First().ErrorMessage));
                return RedirectToRoute(PSSCharacterCertificateDetailsUpdate.UpdateDetails, new { fileNumber = model.FileNumber });
            }
            catch (UserNotAuthorizedForThisActionException)
            {
                _notifier.Add(NotifyType.Warning, PoliceErrorLang.usernotauthorized());
                return Redirect("~/Admin");
            }
            catch (NoRecordFoundException)
            {
                _notifier.Add(NotifyType.Warning, ErrorLang.norecord404());
                return RedirectToRoute(PSSCharacterCertificateDetailsUpdate.SearchFileNumber);
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