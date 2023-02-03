using Orchard;
using Orchard.Logging;
using Orchard.UI.Admin;
using Orchard.UI.Notify;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Lang;
using System;
using System.Web.Mvc;
using Parkway.CBS.Core.HelperModels;
using System.Collections.Generic;
using System.Linq;
using Parkway.CBS.Police.Core.Lang;
using Parkway.CBS.Police.Core.VM;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Admin.RouteName;

namespace Parkway.CBS.Police.Admin.Controllers
{
    [Admin]
    public class PSSAdminUserController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly INotifier _notifier;
        public ILogger Logger { get; set; }
        private readonly IPSSAdminUserHandler _adminUserHandler;


        public PSSAdminUserController(IOrchardServices orchardServices, IPSSAdminUserHandler adminUserHandler)
        {
            _orchardServices = orchardServices;
            _notifier = orchardServices.Notifier;
            Logger = NullLogger.Instance;
            _adminUserHandler = adminUserHandler;
        }

        /// <summary>
        /// Create new user
        /// </summary>
        public ActionResult CreateUser()
        {
            try
            {
                _adminUserHandler.CheckForPermission(Permissions.CanCreateAdminUser);
                AdminUserCreationVM model = _adminUserHandler.GetCreateUserVM();
                return View(model);
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
        /// Create new user
        /// </summary>
        [HttpPost]
        public ActionResult CreateUser(AdminUserCreationVM userInput)
        {
            List<ErrorModel> errors = new List<ErrorModel> { };
            try
            {
                _adminUserHandler.CheckForPermission(Permissions.CanCreateAdminUser);
                _adminUserHandler.CreateAdminUser(ref errors, userInput);
                _notifier.Add(NotifyType.Information, PoliceLang.savesuccessfully);
                return RedirectToRoute(UserManagement.PSSUserReport);
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
            catch (CBSUserAlreadyExistsException)
            {
                _notifier.Add(NotifyType.Error, ErrorLang.profilealreadyexists());
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.genericexception());
                return Redirect("~/Admin");
            }
            AdminUserCreationVM model = _adminUserHandler.GetCreateUserVM(userInput.CommandCategoryId);
            userInput.CommandCategories = model.CommandCategories;
            userInput.RoleTypes = model.RoleTypes;
            userInput.Commands = model.Commands;
            userInput.ServiceTypes = model.ServiceTypes;
            _adminUserHandler.PopulateAdminUserModelForPostback(userInput);
            return View(userInput);
        }


        [HttpGet]
        public ActionResult EditUser(string adminUserId)
        {
           
            try
            {
                if (string.IsNullOrEmpty(adminUserId))
                {
                    throw new ArgumentException($"'{nameof(adminUserId)}' cannot be null or empty.", nameof(adminUserId));
                }

                _adminUserHandler.CheckForPermission(Permissions.CanEditAdminUsers);

                if (!int.TryParse(adminUserId, out int adminUserIdParsed))
                {
                    throw new Exception("Can not parse adminUserId");
                }
                AdminUserCreationVM model = _adminUserHandler.GetEditUserVM(adminUserIdParsed);
                return View(model);
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
        /// edit admin user
        /// </summary>
        [HttpPost]
        public ActionResult EditUser(AdminUserCreationVM userInput)
        {
            List<ErrorModel> errors = new List<ErrorModel> { };
            try
            {
                _adminUserHandler.CheckForPermission(Permissions.CanEditAdminUsers);
                _adminUserHandler.EditAdminUser(ref errors, userInput);
                _notifier.Add(NotifyType.Information, PoliceLang.updatesuccessfull);
                return RedirectToRoute(UserManagement.PSSUserReport);
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
            AdminUserCreationVM model = _adminUserHandler.GetEditUserVM(userInput.AdminUserId);
            userInput.CommandCategories = model.CommandCategories;
            userInput.RoleTypes = model.RoleTypes;
            userInput.Commands = model.Commands;
            userInput.ServiceTypes = model.ServiceTypes;
            userInput.SelectedServiceTypes = model.SelectedServiceTypes;
            userInput.AdminUserType = model.AdminUserType;
            userInput.FlowDefinitionLevels = model.FlowDefinitionLevels;
            userInput.FlowDefinitions = model.FlowDefinitions;
            userInput.SelectedFlowDefinitions = model.SelectedFlowDefinitions;
            userInput.SelectedFlowDefinitionLevels = model.SelectedFlowDefinitionLevels;
            userInput.OfficerSectionCommands = model.OfficerSectionCommands;
            userInput.OfficerSubSectionCommands = model.OfficerSectionCommands;
            return View(userInput);
        }
    }
}