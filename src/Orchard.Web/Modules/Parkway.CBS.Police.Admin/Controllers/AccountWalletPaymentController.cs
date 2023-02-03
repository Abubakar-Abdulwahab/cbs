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
    public class AccountWalletPaymentController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly INotifier _notifier;
        private readonly IAccountWalletPaymentHandler _accountWalletPaymentHandler;
        public ILogger Logger { get; set; }

        public AccountWalletPaymentController(IOrchardServices orchardServices, IAccountWalletPaymentHandler accountWalletPaymentHandler)
        {
            _orchardServices = orchardServices;
            _accountWalletPaymentHandler = accountWalletPaymentHandler;
            _notifier = orchardServices.Notifier;
            Logger = NullLogger.Instance;
        }

        // GET: InitiatePaymentRequest
        public ActionResult InitiatePaymentRequest()
        {
            try
            {
                _accountWalletPaymentHandler.CheckForPermission(Permissions.CanCreateWalletPaymentRequest);
                return View(_accountWalletPaymentHandler.GetInitiateWalletPaymentVM());
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


        // POST: InitiatePaymentRequest
        [HttpPost]
        public ActionResult InitiatePaymentRequest(InitiateAccountWalletPaymentVM userInputModel)
        {
            List<ErrorModel> errors = new List<ErrorModel> { };
            try
            {
                _accountWalletPaymentHandler.CheckForPermission(Permissions.CanCreateWalletPaymentRequest);
                Logger.Information($"About to initiate a payment request from account wallet configuration id {userInputModel.WalletPaymentRequests.FirstOrDefault().SelectedWalletId}. User Id: {_orchardServices.WorkContext.CurrentUser.Id}");

                _accountWalletPaymentHandler.InitiateWalletPaymentRequest(ref errors, userInputModel);
                _notifier.Add(NotifyType.Information, PoliceLang.ToLocalizeString($"Payment request from {userInputModel.WalletPaymentRequests.FirstOrDefault().SelectedWallet} wallet with a Total amount of ₦{userInputModel.WalletPaymentRequests.Sum(x => x.Amount):n2} has been successfully initiated to the following account detail(s).{string.Join(",", userInputModel.WalletPaymentRequests.Select(x => new { AccountInfo = x.AccountName + ": " + x.AccountNumber }).Select(x => x.AccountInfo))}"));
                return RedirectToRoute(AccountWalletPaymentReport.Report);
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

            InitiateAccountWalletPaymentVM model = _accountWalletPaymentHandler.GetInitiateWalletPaymentVM();
            userInputModel.Banks = model.Banks;
            userInputModel.ExpenditureHeads = model.ExpenditureHeads;
            userInputModel.AccountWalletConfigurations = model.AccountWalletConfigurations;

            return View(userInputModel);
        }
    }
}