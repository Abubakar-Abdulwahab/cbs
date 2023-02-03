using Orchard;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Mvc.Extensions;
using Orchard.Security;
using Orchard.UI.Admin;
using Orchard.UI.Notify;
using Orchard.Users.Events;
using Parkway.CBS.Module.Controllers.Handlers.Contracts;
using Parkway.CBS.Core.Exceptions;
using System;
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using System.Web.Mvc;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Orchard.UI.Navigation;

namespace Parkway.CBS.Module.Controllers
{
    [Admin]
    public class SettingsController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly INotifier _notifier;
        private dynamic Shape { get; set; }
        private readonly ISettingsHandler _handler;
        public ILogger Logger { get; set; }
        private readonly IMembershipService _membershipService;
        private readonly IUserEventHandler _userEventHandler;
        public Localizer T { get; set; }

        public SettingsController(IOrchardServices orchardServices, ISettingsHandler handler, IShapeFactory shapeFactory, IMembershipService membershipService,
            IUserEventHandler userEventHandler)
        {
            _orchardServices = orchardServices;
            _notifier = orchardServices.Notifier;
            _handler = handler;
            Shape = shapeFactory;
            Logger = NullLogger.Instance;
            _membershipService = membershipService;
            _userEventHandler = userEventHandler;
            T = NullLocalizer.Instance;
        }


        int MinPasswordLength
        {
            get
            {
                return _membershipService.GetSettings().MinRequiredPasswordLength;
            }
        }


        /// <summary>
        /// Set state
        /// </summary>
        /// <returns>ActionResult</returns>
        public ActionResult SetState()
        {
            try
            {
                Logger.Information("Getting set tenant state view");
                SetStateViewModel model = _handler.SetStateView();
                return View(model);
            }
            #region Catch clauses
            catch (UserNotAuthorizedForThisActionException exception)
            {
                var message = String.Format("\nUser ID {0} tried to manage admin settings without permission", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Warning(exception, exception.Message + message);
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotauthorized());
            }
            catch (CannotConnectToCashFlowException)
            {
                Logger.Error("Cannot connect to cashflow");
                _notifier.Add(NotifyType.Warning, ErrorLang.cannotconnettoinvoicingservice());
            }
            catch (TenantStateHasAlreadyBeenSetException exception)
            {
                Logger.Error(exception, "Tried to set tenant state setting when it has already been set");
                _notifier.Add(NotifyType.Warning, ErrorLang.tenantstatesettingshasalreadybeenset(exception.Message));
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.genericexception());
            }
            #endregion
            return Redirect("~/Admin");
        }


        [HttpPost, ActionName("SetState")]
        public ActionResult SetState(string stateId, string Identifier)
        {
            try
            {
                _handler.TrySaveTenantStateSettings(this, stateId, Identifier);
                _notifier.Information(Lang.savesuccessfully);
                return RedirectToAction("ListOfExpertSystems", "Settings", new { });
            }
            #region Catch clauses
            catch (UserNotAuthorizedForThisActionException exception)
            {
                var message = String.Format("\nUser ID {0} tried to manage admin settings without permission", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Information(exception, exception.Message + message);
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotauthorized());
            }
            catch (AuthorizedUserNotFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotfound());
            }
            catch (DirtyFormDataException exception)
            {
                var message = String.Format("\nInvalid model state  - LastUpdatedBy ID  {0}", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Information(exception, exception.Message + message);
                var model = new SetStateViewModel() { States = _handler.ListOfStates(null).Select(st => new Core.Models.TenantCBSSettings { StateId = st.Id, StateName = st.Name }).ToList() };
                return View(model);
            }
            catch (TenantStateHasAlreadyBeenSetException exception)
            {
                Logger.Error(exception, "Tried to set tenant state setting when it has already been set");
                _notifier.Add(NotifyType.Warning, ErrorLang.tenantstatesettingshasalreadybeenset(exception.Message));
            }
            catch (CouldNotParseStringValueException exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Warning, ErrorLang.invalidinputtype());
                var model = new SetStateViewModel() { States = _handler.ListOfStates(null).Select(st => new Core.Models.TenantCBSSettings { StateId = st.Id, StateName = st.Name }).ToList() };
                return View(model);
            }
            catch (CannotFindTenantIdentifierException exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Warning, ErrorLang.cannotfindstate());
                var model = new SetStateViewModel() { States = _handler.ListOfStates(null).Select(st => new Core.Models.TenantCBSSettings { StateId = st.Id, StateName = st.Name }).ToList() };
                return View(model);
            }
            catch (CannotConnectToCashFlowException)
            {
                Logger.Error("Cannot connect to cashflow");
                _notifier.Add(NotifyType.Warning, ErrorLang.cannotconnettoinvoicingservice());
            }
            catch (CannotSaveTenantSettings)
            {
                Logger.Error("Cannot save tenant setting");
                _notifier.Add(NotifyType.Error, ErrorLang.cannotsavetenantsettingsinfo());
                var model = new SetStateViewModel() { States = _handler.ListOfStates(null).Select(st => new Core.Models.TenantCBSSettings { StateId = st.Id, StateName = st.Name }).ToList() };
                return View(model);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Warning, ErrorLang.genericexception());
            }
            #endregion
            return Redirect("~/Admin");
        }



        /// <summary>
        /// Show admin settings page
        /// </summary>
        /// <returns>ActionResult</returns>
        public ActionResult ListOfExpertSystems(PagerParameters pagerParameters)
        {
            try
            {
                var pager = new Pager(_orchardServices.WorkContext.CurrentSite, pagerParameters);
                int skip = pagerParameters.Page.HasValue ? (pagerParameters.Page.Value - 1) * 10 : 0;
                int take = pagerParameters.PageSize.HasValue ? pagerParameters.PageSize.Value : 10;
                Logger.Information("Getting view for list of expert systems");
                ExpertSystemListViewModel model = _handler.GetListOfExpertSystemsView(skip, take);
                var pageShape = Shape.Pager(pager).TotalItemCount(model.PagerSize);
                model.Pager = pageShape;
                return View(model);
            }
            #region Catch clauses
            catch (UserNotAuthorizedForThisActionException exception)
            {
                var message = String.Format("\nUser ID {0} tried to manage admin settings without permission", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Warning(exception, exception.Message + message);
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotauthorized());
            }
            catch (CannotConnectToCashFlowException)
            {
                Logger.Error("Cannot connect to cashflow");
                _notifier.Add(NotifyType.Warning, ErrorLang.cannotconnettoinvoicingservice());
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.genericexception());
            }
            #endregion
            return Redirect("~/Admin");
        }


        /// <summary>
        /// Get list of ref data attached to this state
        /// </summary>
        /// <param name="stateName"></param>
        /// <returns>JsonResult</returns>
        public JsonResult GetRegsiteredRefDataItems(string stateName)
        {
            Logger.Information("Getting list of ref data");
            var refData = _handler.GetRegisteredRefData(stateName);
            return Json(refData, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Get list of ref data attached to this state
        /// </summary>
        /// <param name="stateName"></param>
        /// <returns>JsonResult</returns>
        public JsonResult GetClientSecret(string clientId)
        {
            Logger.Information("Getting list of ref data");
            var secret = _handler.GetClientSecret(clientId);
            if (string.IsNullOrEmpty(secret)) { secret = ""; }
            return Json(secret, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Show admin settings page
        /// </summary>
        /// <returns>ActionResult</returns>
        public ActionResult CreateExpertSystem()
        {
            try
            {
                Logger.Information("Creating new expert system");
                ExpertSettingsViewModel model = _handler.CreateExpertSystemView();
                return View(model);
            }
            #region Catch clauses
            catch (UserNotAuthorizedForThisActionException exception)
            {
                var message = String.Format("\nUser ID {0} tried to manage admin settings without permission", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Warning(exception, exception.Message + message);
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotauthorized());
            }
            catch (CannotConnectToCashFlowException exception)
            {
                Logger.Error(exception, "Cannot connect to cashflow");
                _notifier.Add(NotifyType.Warning, ErrorLang.cannotconnettoinvoicingservice());
            }
            catch (CannotFindTenantIdentifierException exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Warning, ErrorLang.tenantidentifierhasnotbeenset());
                return RedirectToAction("ListOfExpertSystems", "Settings", new { });
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.genericexception());
            }
            #endregion
            return Redirect("~/Admin");
        }


        [HttpPost, ActionName("CreateExpertSystem")]
        public ActionResult CreateExpertSystem(ExpertSettingsViewModel model, string bank)
        {
            try
            {
                model = _handler.TrySaveNewExpertSystem(this, model.ExpertSystemsSettings, HttpContext.Request.Files, bank);
                _notifier.Information(Lang.savesuccessfully);
                return RedirectToAction("ListOfExpertSystems", "Settings", new { });
            }
            #region Catch clauses
            catch (UserNotAuthorizedForThisActionException exception)
            {
                var message = String.Format("\nUser ID {0} tried to manage admin settings without permission", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Information(exception, exception.Message + message);
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotauthorized());
            }
            catch(CannotFindTenantIdentifierException exception)
            {
                _notifier.Add(NotifyType.Error, ErrorLang.tenantidentifierhasnotbeenset());
                Logger.Error(exception, "Error getting tenant state settings " + exception.Message);
            }
            catch (AuthorizedUserNotFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotfound());
            }
            catch (DirtyFormDataException exception)
            {
                var message = String.Format("\nInvalid model state  - LastUpdatedBy ID  {0}", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Information(exception, exception.Message + message);
                TenantCBSSettings tenant = _handler.GetTenantSettings();
                model.ListOfRefData = _handler.GetRegisteredRefData(tenant.Identifier);
                model.Banks = _handler.ListOfBanks(null);
                model.States = new System.Collections.Generic.List<Cashflow.Ng.Models.CashFlowState> { { new Cashflow.Ng.Models.CashFlowState { Name = tenant.StateName } } };
                return View(model);
            }
            catch (CannotConnectToCashFlowException)
            {
                Logger.Error("Cannot connect to cashflow");
                _notifier.Add(NotifyType.Warning, ErrorLang.cannotconnettoinvoicingservice());
            }
            catch (CannotSaveTenantSettings exception)
            {
                Logger.Error(exception, "Cannot save tenant setting");
                _notifier.Add(NotifyType.Error, ErrorLang.cannotsavetenantsettingsinfo());
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Warning, ErrorLang.genericexception());
            }
            #endregion
            return Redirect("~/Admin");
        }


        public ActionResult EditExpertSystem(string identifier)
        {
            try
            {
                Logger.Information("Creating new expert system");
                ExpertSettingsViewModel model = _handler.EditExpertSystemView(identifier);
                return View("CreateExpertSystem", model);
            }
            #region Catch clauses
            catch (UserNotAuthorizedForThisActionException exception)
            {
                var message = String.Format("\nUser ID {0} tried to manage admin settings without permission", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Warning(exception, exception.Message + message);
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotauthorized());
            }
            catch (CannotConnectToCashFlowException exception)
            {
                Logger.Error(exception, "Cannot connect to cashflow");
                _notifier.Add(NotifyType.Warning, ErrorLang.cannotconnettoinvoicingservice());
            }
            catch (CannotFindTenantIdentifierException exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Warning, ErrorLang.tenantidentifierhasnotbeenset());
                return RedirectToAction("ListOfExpertSystems", "Settings", new { });
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.genericexception());
            }
            #endregion
            return Redirect("~/Admin");
        }



        [HttpPost, ActionName("EditExpertSystem")]
        public ActionResult EditExpertSystem(ExpertSettingsViewModel model, string bank, string identifier)
        {
            try
            {
                Logger.Information("Creating new expert system");
                ExpertSettingsViewModel modele = _handler.TryUpdateExpertSystemSettings(identifier, this, model, HttpContext.Request.Files, bank);
                return View("CreateExpertSystem", modele);
            }
            #region Catch clauses
            catch (UserNotAuthorizedForThisActionException exception)
            {
                var message = String.Format("\nUser ID {0} tried to manage admin settings without permission", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Warning(exception, exception.Message + message);
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotauthorized());
            }
            catch (CannotConnectToCashFlowException exception)
            {
                Logger.Error(exception, "Cannot connect to cashflow");
                _notifier.Add(NotifyType.Warning, ErrorLang.cannotconnettoinvoicingservice());
            }
            catch (CannotFindTenantIdentifierException exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Warning, ErrorLang.tenantidentifierhasnotbeenset());
                return RedirectToAction("ListOfExpertSystems", "Settings", new { });
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.genericexception());
            }
            #endregion
            return Redirect("~/Admin");
        }


        public ActionResult ReferenceDataSettings()
        {
            try
            {
                ReferenceDataViewModel model = _handler.GetReferenceDataSettingsView();
                return View(model);
            }
            #region catch clauses
            catch (TenantNotFoundException exception)
            {
                Logger.Error(exception, ErrorLang.tenant404().ToString());
                _notifier.Add(NotifyType.Error, ErrorLang.tenant404());
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
            }
            #endregion
            return Redirect("~/Admin");
        }


        [HttpPost, ActionName("ReferenceDataSettings")]
        public ActionResult ReferenceDataSettings(ReferenceDataViewModel model)
        {
            try
            {
                _handler.TrySaveReferenceDataSettings(model);
                return View(model);
            }
            #region catch clauses
            catch (TenantNotFoundException exception)
            {
                Logger.Error(exception, ErrorLang.tenant404().ToString());
                _notifier.Add(NotifyType.Error, ErrorLang.tenant404());
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
            }
            #endregion
            return Redirect("~/Admin");
        }

        // GET: Settings
        public ActionResult ChangePassword()
        {
            ViewData["PasswordLength"] = _membershipService.GetSettings().MinRequiredPasswordLength;
            return View(_handler.ChangePasswordView());
        }



        [Authorize]
        [HttpPost]
        [AlwaysAccessible]
        [ValidateInput(false)]
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Exceptions result in password not being changed.")]
        public ActionResult ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            ViewData["PasswordLength"] = MinPasswordLength;

            if (!ValidateChangePassword(currentPassword, newPassword, confirmPassword))
            {
                return View();
            }

            try
            {
                var validated = _membershipService.ValidateUser(User.Identity.Name, currentPassword);

                if (validated != null)
                {
                    _membershipService.SetPassword(validated, newPassword);
                    _userEventHandler.ChangedPassword(validated);
                    _notifier.Information(Lang.passwordchangedsuccessfully);
                    return View();
                }

                ModelState.AddModelError("_FORM", T("The current password is incorrect or the new password is invalid."));
                return ChangePassword();
            }
            catch
            {
                ModelState.AddModelError("_FORM", T("The current password is incorrect or the new password is invalid."));
                return ChangePassword();
            }
        }

        #region Validation Methods

        private bool ValidateChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            if (String.IsNullOrEmpty(currentPassword))
            {
                ModelState.AddModelError("currentPassword", T("You must specify a current password."));
            }
            if (newPassword == null || newPassword.Length < MinPasswordLength)
            {
                ModelState.AddModelError("newPassword", T("You must specify a new password of {0} or more characters.", MinPasswordLength));
            }

            if (!String.Equals(newPassword, confirmPassword, StringComparison.Ordinal))
            {
                ModelState.AddModelError("_FORM", T("The new password and confirmation password do not match."));
            }

            return ModelState.IsValid;
        }

        #endregion
    }
}