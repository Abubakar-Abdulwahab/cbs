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
    public class CommandWalletController : Controller
    {
        private readonly IOrchardServices _orchardServices;

        private readonly ICommandWalletHandler _commandWalletHandler;

        private readonly INotifier _notifier;
        public ILogger Logger { get; set; }



        public CommandWalletController(IOrchardServices orchardServices, INotifier notifier, ICommandWalletHandler commandWalletHandler)
        {
            _orchardServices = orchardServices;
            _notifier = notifier;
            Logger = NullLogger.Instance;
            _commandWalletHandler = commandWalletHandler;
        }

        // GET: AddCommandWallet
        public ActionResult AddCommandWallet()
        {
            try
            {
                _commandWalletHandler.CheckForPermission(Permissions.CanCreateCommandWallet);
                AddCommandWalletVM model = _commandWalletHandler.GetAddCommandWalletVM();
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

        // POST: AddCommandWallet
        [HttpPost]
        public ActionResult AddCommandWallet(AddCommandWalletVM userInputModel)
        {
            List<ErrorModel> errors = new List<ErrorModel> { };
            try
            {
                Logger.Information($"About to create wallet. Wallet number {userInputModel.WalletNumber}. Command Id: {userInputModel.SelectedCommandId}. User Id: {_orchardServices.WorkContext.CurrentUser.Id}");
                _commandWalletHandler.CheckForPermission(Permissions.CanCreateCommandWallet);
                _commandWalletHandler.AddCommandWallet(ref errors, userInputModel);
                _notifier.Add(NotifyType.Information, PoliceLang.savesuccessfully);
                return RedirectToRoute(CommandWallet.PSSCommandWallet);
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

            AddCommandWalletVM model = _commandWalletHandler.GetAddCommandWalletVM();
            userInputModel.Commands = model.Commands;
            userInputModel.Banks = model.Banks;

            return View(userInputModel);
        }
    }
}