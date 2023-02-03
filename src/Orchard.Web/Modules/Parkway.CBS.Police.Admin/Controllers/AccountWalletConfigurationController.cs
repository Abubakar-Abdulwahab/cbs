using Orchard;
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
    public class AccountWalletConfigurationController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly INotifier _notifier;
        private readonly IAccountWalletConfigurationHandler _accountWalletConfigurationHandler;
        public ILogger Logger { get; set; }

        public AccountWalletConfigurationController(IOrchardServices orchardServices, IAccountWalletConfigurationHandler accountWalletConfigurationHandler)
        {
            _orchardServices = orchardServices;
            _notifier = orchardServices.Notifier;
            Logger = NullLogger.Instance;
            _accountWalletConfigurationHandler = accountWalletConfigurationHandler;
        }

        // GET: AddWalletConfiguration
        public ActionResult AddWalletConfiguration(int walletAcctId)
        {
            try
            {
                if (walletAcctId <= 0)
                {
                    _notifier.Add(NotifyType.Error, PoliceLang.nowalletidfound);
                    return RedirectToRoute(AccountsWalletReport.Report);
                }
                _accountWalletConfigurationHandler.CheckForPermission(Permissions.CanAddWalletConfiguration);
                return View(_accountWalletConfigurationHandler.GetAddOrRemoveWalletConfigurationVM(walletAcctId));
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
        // POST: AddWalletConfiguration
        public ActionResult AddWalletConfiguration(AddOrRemoveAccountWalletConfigurationVM userInputModel)
        {
            List<ErrorModel> errors = new List<ErrorModel> { };

            try
            {
                Logger.Information($"User id {_orchardServices.WorkContext.CurrentUser.Id} about to configure user(s) against the account wallet id {userInputModel.AccountWalletId}");
                _accountWalletConfigurationHandler.CheckForPermission(Permissions.CanAddWalletConfiguration);
                _accountWalletConfigurationHandler.AddOrRemoveWalletConfiguration(ref errors, userInputModel);

                if (userInputModel?.RemovedWalletUsers?.Count > 0)
                {
                    _notifier.Add(NotifyType.Information, PoliceLang.ToLocalizeString($"The following user(s) has been successfully removed. {string.Join(",", userInputModel.RemovedWalletUsers.Select(x => new { WalletUserInfo = x.Username + " in " + x.CommandName + " with the role " + x.FlowDefintionLevelName }).Select(x => x.WalletUserInfo))}"));
                }

                if (userInputModel?.AddedWalletUsers?.Count > 0)
                {
                    _notifier.Add(NotifyType.Information, PoliceLang.ToLocalizeString($"The following user(s) has been successfully added. {string.Join(",", userInputModel.AddedWalletUsers.Select(x => new { WalletUserInfo = x.Username + " in " + x.CommandName + " with the role " + x.FlowDefintionLevelName }).Select(x => x.WalletUserInfo))}"));
                }

                return RedirectToRoute(AccountsWalletReport.Report);
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
            AddOrRemoveAccountWalletConfigurationVM model = _accountWalletConfigurationHandler.GetAddOrRemoveWalletConfigurationVM(userInputModel.AccountWalletId);
            userInputModel.FlowDefinitionLevels = model.FlowDefinitionLevels;
            userInputModel.WalletName = model.WalletName;
            return View(userInputModel);

        }

    }
}