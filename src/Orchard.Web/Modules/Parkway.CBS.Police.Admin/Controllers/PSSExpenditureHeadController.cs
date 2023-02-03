using Orchard.Logging;
using Orchard.UI.Admin;
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
using System.Web.Mvc;

namespace Parkway.CBS.Police.Admin.Controllers
{
    [Admin]
    public class PSSExpenditureHeadController : Controller
    {
        private readonly IExpenditureHeadHandler _expenditureHeadHandler;

        private readonly INotifier _notifier;
        public ILogger Logger { get; set; }

        public PSSExpenditureHeadController(INotifier notifier, IExpenditureHeadHandler expenditureHeadHandler)
        {
            _notifier = notifier;
            Logger = NullLogger.Instance;
            _expenditureHeadHandler = expenditureHeadHandler;
        }

        // GET: AddExpenditureHead
        public ActionResult AddExpenditureHead()
        {
            try
            {
                _expenditureHeadHandler.CheckForPermission(Permissions.CanAddExpenditureHead);
                return View(_expenditureHeadHandler.GetAddExpenditureHeadVM());
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

        // POST: AddExpenditureHead
        [HttpPost]
        public ActionResult AddExpenditureHead(AddExpenditureHeadVM userInputModel)
        {
            List<ErrorModel> errors = new List<ErrorModel> { };
            try
            {
                _expenditureHeadHandler.CheckForPermission(Permissions.CanAddExpenditureHead);
                _expenditureHeadHandler.AddExpenditureHead(ref errors, userInputModel);
                _notifier.Add(NotifyType.Information, PoliceLang.savesuccessfully);
                return RedirectToRoute(PSSExpenditureHeadReport.Report);
            }
            catch (DirtyFormDataException exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.ToLocalizeString(errors.First().ErrorMessage));
            }
            catch (CouldNotSaveRecord exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.couldnotsaverecord());
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

            return View(userInputModel);
        }

        // GET: EnableExpenditureHead
        public ActionResult EnableExpenditureHead(int id)
        {
            try
            {
                _expenditureHeadHandler.CheckForPermission(Permissions.CanAddExpenditureHead);
                _expenditureHeadHandler.ToggleIsActiveExpenditureHead(id, isActive: true);
                _notifier.Add(NotifyType.Information, PoliceLang.updatesuccessfull);
                return RedirectToRoute(PSSExpenditureHeadReport.Report);
            }
            catch (NoRecordFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.record404());
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

        // GET: DisableExpenditureHead
        public ActionResult DisableExpenditureHead(int id)
        {
            try
            {
                _expenditureHeadHandler.CheckForPermission(Permissions.CanAddExpenditureHead);
                _expenditureHeadHandler.ToggleIsActiveExpenditureHead(id, isActive: false);
                _notifier.Add(NotifyType.Information, PoliceLang.updatesuccessfull);
                return RedirectToRoute(PSSExpenditureHeadReport.Report);
            }
            catch (NoRecordFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.record404());
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

        // GET: EditExpenditureHead
        public ActionResult EditExpenditureHead(int id)
        {
            try
            {
                _expenditureHeadHandler.CheckForPermission(Permissions.CanAddExpenditureHead);
                return View(_expenditureHeadHandler.GetEditExpenditureHeadVM(id));
            }
            catch (NoRecordFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.record404());
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

        // POST: EditExpenditureHead
        [HttpPost]
        public ActionResult EditExpenditureHead(AddExpenditureHeadVM userInputModel)
        {
            List<ErrorModel> errors = new List<ErrorModel> { };
            try
            {
                _expenditureHeadHandler.CheckForPermission(Permissions.CanAddExpenditureHead);
                _expenditureHeadHandler.EditExpenditureHead(ref errors, userInputModel);
                _notifier.Add(NotifyType.Information, PoliceLang.updatesuccessfully($"{userInputModel.Name} expenditure"));
                return RedirectToRoute(PSSExpenditureHeadReport.Report);
            }
            catch (NoRecordFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.record404());
                return Redirect("~/Admin");
            }
            catch (DirtyFormDataException exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.ToLocalizeString(errors.First().ErrorMessage));
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

            return View(userInputModel);
        }
    }
}