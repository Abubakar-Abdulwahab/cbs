using Orchard;
using Orchard.DisplayManagement;
using Orchard.Logging;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Admin.VM;
using Parkway.CBS.Police.Core.Lang;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace Parkway.CBS.Police.Admin.Controllers
{
    [Admin]
    public class AccountsWalletReportController : Controller
    {
        private readonly IOrchardServices _orchardServices;

        private readonly INotifier _notifier;
        public ILogger Logger { get; set; }
        dynamic Shape { get; set; }

        private readonly IAccountsWalletReportHandler _accountsWalletReportHandler;

        public AccountsWalletReportController(IOrchardServices orchardServices, INotifier notifier, IAccountsWalletReportHandler accountsWalletReportHandler, IShapeFactory shapeFactory)
        {
            _orchardServices = orchardServices;
            _notifier = notifier;
            Logger = NullLogger.Instance;
            _accountsWalletReportHandler = accountsWalletReportHandler;
            Shape = shapeFactory;
        }

        // GET: AccountsWalletReport
        public ActionResult Report(AccountsWalletReportVM userInputModel, PagerParameters pagerParameters)
        {
            try
            {
                _accountsWalletReportHandler.CheckForPermission(Permissions.CanViewAccountWalletReport);

                var pager = new Pager(_orchardServices.WorkContext.CurrentSite, pagerParameters);

                int take = pagerParameters.PageSize ?? 10;

                if (take == 0) { take = 10; pager.PageSize = 10; }
                int skip = pagerParameters.Page.HasValue ? (pagerParameters.Page.Value - 1) * take : 0;

                AccountWalletReportSearchParams searchParams = new AccountWalletReportSearchParams
                {
                    Take = take,
                    Skip = skip,
                    AccountName = userInputModel.AccountName?.Trim(),
                    AccountNumber = userInputModel.AccountNumber?.Trim(),
                    BankId = userInputModel.BankId,
                };

                var vm = _accountsWalletReportHandler.GetVMForReports(searchParams);

                var pageShape = Shape.Pager(pager).TotalItemCount(vm.TotalAccountWalletRecord);
                pageShape.RouteData(DoRouteDataForAccountWalletReport(vm));

                vm.Pager = pageShape;

                return View(vm);
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

        private object DoRouteDataForAccountWalletReport(AccountsWalletReportVM model)
        {
            RouteData routeData = new RouteData();
            if (!string.IsNullOrEmpty(model.AccountNumber)) { routeData.Values.Add("AccountNumber", model.AccountNumber); }
            if (!string.IsNullOrEmpty(model.AccountName)) { routeData.Values.Add("AccountName", model.AccountName); }
            if (model.BankId != 0) { routeData.Values.Add("BankId", model.BankId); }
            return routeData;
        }

    }
}