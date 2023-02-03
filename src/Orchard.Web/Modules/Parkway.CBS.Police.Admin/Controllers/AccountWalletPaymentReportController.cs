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
using System.Globalization;
using System.Web.Mvc;
using System.Web.Routing;

namespace Parkway.CBS.Police.Admin.Controllers
{
    [Admin]
    public class AccountWalletPaymentReportController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly INotifier _notifier;
        private readonly IAccountWalletPaymentReportHandler _accountWalletPaymentReportHandler;
        public ILogger Logger { get; set; }
        dynamic Shape { get; set; }
        public AccountWalletPaymentReportController(IShapeFactory shapeFactory, IOrchardServices orchardServices, IAccountWalletPaymentReportHandler accountWalletPaymentReportHandler)
        {
            _orchardServices = orchardServices;
            _notifier = orchardServices.Notifier;
            Logger = NullLogger.Instance;
            Shape = shapeFactory;
            _accountWalletPaymentReportHandler = accountWalletPaymentReportHandler;
        }

        // GET: AccountWalletPaymentReport
        public ActionResult Report(AccountWalletPaymentReportVM userInputModel, PagerParameters pagerParameters)
        {
            try
            {
                _accountWalletPaymentReportHandler.CheckForPermission(Permissions.CanViewWalletPaymentReport);

                var pager = new Pager(_orchardServices.WorkContext.CurrentSite, pagerParameters);

                DateTime startDate = DateTime.Now.Date.AddMonths(-3);
                DateTime endDate = DateTime.Now;

                if (!string.IsNullOrEmpty(userInputModel.From) && !string.IsNullOrEmpty(userInputModel.End))
                {
                    try
                    {
                        startDate = DateTime.ParseExact(userInputModel.From, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        endDate = DateTime.ParseExact(userInputModel.End, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
                //get the date up until the final sec
                endDate = endDate.Date.AddDays(1).AddMilliseconds(-1);

                int take = pagerParameters.PageSize ?? 10;

                if (take == 0) { take = 10; pager.PageSize = 10; }
                int skip = pagerParameters.Page.HasValue ? (pagerParameters.Page.Value - 1) * take : 0;

                AccountWalletPaymentSearchParams searchParams = new AccountWalletPaymentSearchParams
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    Skip = skip,
                    UserPartRecordId = _orchardServices.WorkContext.CurrentUser.Id,
                    Take = take,
                    PaymentId = userInputModel.PaymentId?.Trim(),
                    SourceAccountName = userInputModel.SourceAccount?.Trim(),
                    Status = userInputModel.Status,
                    BeneficiaryAccoutNumber = userInputModel.BeneficiaryAccountNumber?.Trim(),
                    ExpenditureHeadId = userInputModel.ExpenditureHeadId
                };

                var vm = _accountWalletPaymentReportHandler.GetAccountWalletPaymentReportVM(searchParams);

                var pageShape = Shape.Pager(pager).TotalItemCount(vm.TotalAccountWalletPaymentReportRecord);
                pageShape.RouteData(DoRouteDataForAccountWalletPaymentReport(vm));

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

        private object DoRouteDataForAccountWalletPaymentReport(AccountWalletPaymentReportVM model)
        {
            RouteData routeData = new RouteData();
            if (!string.IsNullOrEmpty(model.SourceAccount)) { routeData.Values.Add("SourceAccount", model.SourceAccount); }
            if (!string.IsNullOrEmpty(model.PaymentId)) { routeData.Values.Add("PaymentId", model.PaymentId); }
            if (!string.IsNullOrEmpty(model.BeneficiaryAccountNumber)) { routeData.Values.Add("BeneficiaryAccountNumber", model.BeneficiaryAccountNumber); }
            if (model.ExpenditureHeadId > 0) { routeData.Values.Add("ExpenditureHeadId", model.ExpenditureHeadId); }
            if (model.Status > 0) { routeData.Values.Add("Status", model.Status); }
            return routeData;
        }

    }
}