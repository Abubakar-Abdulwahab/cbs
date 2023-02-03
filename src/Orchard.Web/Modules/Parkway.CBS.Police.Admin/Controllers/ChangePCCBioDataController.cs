using Orchard;
using Orchard.Logging;
using Orchard.UI.Admin;
using Orchard.UI.Notify;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Admin.VM;
using Parkway.CBS.Police.Core.Lang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Parkway.CBS.Police.Admin.Controllers
{
    [Admin]
    public class ChangePCCBioDataController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly INotifier _notifier;
        private readonly IChangePCCBioDataHandler _handler;
        public ILogger Logger { get; set; }

        public ChangePCCBioDataController(IOrchardServices orchardServices, IChangePCCBioDataHandler handler)
        {
            _orchardServices = orchardServices;
            _notifier = orchardServices.Notifier;
            _handler = handler;
            Logger = NullLogger.Instance;

        }


        [HttpGet]
        public ActionResult ChangeBioData()
        {
            try
            {
                _handler.CheckForPermission(Permissions.CanChangePassportBioDataPageForPCC);
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
                _notifier.Add(NotifyType.Error, PoliceErrorLang.genericexception());
                return Redirect("~/Admin");
            }
        }


        [HttpPost]
        public ActionResult ChangeBioData(ChangeBioDataVM userInput)
        {
            List<ErrorModel> errors = new List<ErrorModel> { };

            try
            {
                _handler.CheckForPermission(Permissions.CanChangePassportBioDataPageForPCC);
                if (string.IsNullOrEmpty(userInput.FileNumber?.Trim()))
                {
                    errors.Add(new ErrorModel { FieldName = nameof(ChangeBioDataVM.FileNumber), ErrorMessage = "File number is required" });
                }

                Logger.Information($"Changing bio data for PCC with file number {userInput.FileNumber}. User Id: {_orchardServices.WorkContext.CurrentUser.Id}");
                if (HttpContext.Request.Files.Get("passportBioDataPageFile") == null || HttpContext.Request.Files.Get("passportBioDataPageFile").ContentLength == 0)
                {
                    Logger.Error($"Bio data page file for pcc with file number {userInput.FileNumber} not uploaded");
                    errors.Add(new ErrorModel { FieldName = nameof(ChangeBioDataVM.FileNumber), ErrorMessage = $"Bio data page file for pcc with file number {userInput.FileNumber} not uploaded" });
                }

                if(errors.Count() > 0) { throw new DirtyFormDataException(); }

                Logger.Information($"Processing bio data page update for pcc with file number {userInput.FileNumber} User Id: {_orchardServices.WorkContext.CurrentUser.Id}");
                if (!_handler.ProcessBioDataUpdate(userInput.FileNumber.Trim(), HttpContext.Request.Files.Get("passportBioDataPageFile"), ref errors))
                {
                    _notifier.Add(NotifyType.Error, PoliceLang.ToLocalizeString($"Error updating passport bio data page for {userInput.FileNumber}"));
                }
                else
                {
                    _notifier.Add(NotifyType.Information, PoliceLang.ToLocalizeString($"Passport bio data page for {userInput.FileNumber} updated succesfully"));
                }
                return RedirectToRoute(RouteName.ChangePCCBioData.ChangeBioData);
            }
            catch (DirtyFormDataException)
            {
                Logger.Error("Unable to change pcc bio data page due to the following error(s):");
                foreach (var error in errors)
                {
                    Logger.Error(error.ErrorMessage);
                    ModelState.AddModelError(error.FieldName, error.ErrorMessage);
                }
                return View(userInput);
            }
            catch (UserNotAuthorizedForThisActionException)
            {
                _notifier.Add(NotifyType.Warning, PoliceErrorLang.usernotauthorized());
                return Redirect("~/Admin");
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, PoliceErrorLang.genericexception());
                return Redirect("~/Admin");
            }
        }
    }
}