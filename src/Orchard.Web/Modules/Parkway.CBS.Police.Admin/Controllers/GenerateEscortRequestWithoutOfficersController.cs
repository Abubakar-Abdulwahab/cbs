using Orchard;
using Orchard.DisplayManagement;
using Orchard.Logging;
using Orchard.UI.Admin;
using Orchard.UI.Notify;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Admin.VM;
using Parkway.CBS.Police.Core.Exceptions;
using Parkway.CBS.Police.Core.Lang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Parkway.CBS.Police.Admin.Controllers
{
    [Admin]
    public class GenerateEscortRequestWithoutOfficersController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly INotifier _notifier;
        private readonly IGenerateEscortRequestWithoutOfficersHandler _handler;
        public ILogger Logger { get; set; }
        dynamic Shape { get; set; }

        public GenerateEscortRequestWithoutOfficersController(IOrchardServices orchardServices, IShapeFactory shapeFactory, IGenerateEscortRequestWithoutOfficersHandler handler)
        {
            _orchardServices = orchardServices;
            _notifier = orchardServices.Notifier;
            _handler = handler;
            Logger = NullLogger.Instance;
            Shape = shapeFactory;
        }


        [HttpGet]
        public ActionResult GenerateEscortRequestForDefaultBranchWithoutOfficers(Int64 batchId)
        {
            try
            {
                _handler.CheckForPermission(Permissions.CanViewBranchProfileDetail);
                if (batchId <= 0)
                {
                    _notifier.Add(NotifyType.Error, ErrorLang.ToLocalizeString("Invalid file upload batch id"));
                    return Redirect("~/Admin");
                }
                return View(_handler.GetGenerateEscortRequestVM(batchId));
            }
            catch (UserNotAuthorizedForThisActionException)
            {
                _notifier.Add(NotifyType.Warning, PoliceErrorLang.usernotauthorized());
                return Redirect("~/Admin");
            }
            catch (PSSRequestAlreadyExistsException)
            {
                Logger.Error($"A request has already been generated");
                _notifier.Add(NotifyType.Information, PoliceErrorLang.ToLocalizeString("A request has already been generated"));
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
        public ActionResult GenerateEscortRequestForDefaultBranchWithoutOfficers(GenerateEscortRequestForWithoutOfficersVM userInputModel)
        {
            List<ErrorModel> errors = new List<ErrorModel> { };

            try
            {
                _handler.CheckForPermission(Permissions.CanViewBranchProfileDetail);

                var response = _handler.GenerateEscortRequestForDefaultBranch(userInputModel, ref errors);
                _notifier.Add(NotifyType.Information, PoliceLang.ToLocalizeString($"A request with invoice number {response.InvoiceNumber} has been generated successfully."));
                return RedirectToRoute(RouteName.GenerateRequestWithoutOfficers.GenerateRequestForDefaultBranchWithoutOfficersUpload, new { profileId = response.PayerId });
            }
            catch (DirtyFormDataException exception)
            {
                Logger.Error(exception, exception.Message);
                foreach (var error in errors)
                {
                    ModelState.AddModelError(error.FieldName, error.ErrorMessage);
                }
                _handler.PopulateGenerateEscortRequestVMForPostback(userInputModel);

                return View(userInputModel);
            }
            catch (UserNotAuthorizedForThisActionException exception)
            {
                Logger.Error(exception, exception.Message);
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
        public ActionResult GenerateEscortRequestForBranchWithoutOfficers(Int64 batchId)
        {
            try
            {
                _handler.CheckForPermission(Permissions.CanViewBranchProfileDetail);
                if (batchId <= 0)
                {
                    _notifier.Add(NotifyType.Error, ErrorLang.ToLocalizeString("Invalid file upload batch id"));
                    return Redirect("~/Admin");
                }
                return View(_handler.GetGenerateEscortRequestVM(batchId));
            }
            catch (UserNotAuthorizedForThisActionException)
            {
                _notifier.Add(NotifyType.Warning, PoliceErrorLang.usernotauthorized());
                return Redirect("~/Admin");
            }
            catch (PSSRequestAlreadyExistsException)
            {
                Logger.Error($"A request has already been generated for the branch");
                _notifier.Add(NotifyType.Information, PoliceErrorLang.ToLocalizeString("A request has already been generated for the branch"));
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
        public ActionResult GenerateEscortRequestForBranchWithoutOfficers(GenerateEscortRequestForWithoutOfficersVM userInputModel)
        {
            List<ErrorModel> errors = new List<ErrorModel> { };

            try
            {
                _handler.CheckForPermission(Permissions.CanViewBranchProfileDetail);

                var response = _handler.GenerateEscortRequestForBranch(userInputModel, ref errors);
                _notifier.Add(NotifyType.Information, PoliceLang.ToLocalizeString($"A request with invoice number {response.InvoiceNumber} has been generated successfully."));
                return RedirectToRoute(RouteName.GenerateRequestWithoutOfficers.GenerateRequestForBranchWithoutOfficersUpload, new { branchId = userInputModel.BranchDetails.Id});
            }
            catch (DirtyFormDataException exception)
            {
                Logger.Error(exception, exception.Message);
                foreach (var error in errors)
                {
                    ModelState.AddModelError(error.FieldName, error.ErrorMessage);
                }
                _handler.PopulateGenerateEscortRequestVMForPostback(userInputModel);

                return View(userInputModel);
            }
            catch (UserNotAuthorizedForThisActionException exception)
            {
                Logger.Error(exception, exception.Message);
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