using Orchard;
using Orchard.DisplayManagement;
using Orchard.Logging;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Admin.RouteName;
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
    public class AccountWalletPaymentApprovalController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly INotifier _notifier;
        private readonly IAccountWalletPaymentApprovalHandler _accountWalletPaymentApprovalHandler;
        public ILogger Logger { get; set; }
        dynamic Shape { get; set; }

        public AccountWalletPaymentApprovalController(IShapeFactory shapeFactory, IOrchardServices orchardServices, IAccountWalletPaymentApprovalHandler accountWalletPaymentApprovalHandler)
        {
            _orchardServices = orchardServices;
            _notifier = orchardServices.Notifier;
            Logger = NullLogger.Instance;
            Shape = shapeFactory;
            _accountWalletPaymentApprovalHandler = accountWalletPaymentApprovalHandler;
        }

        // GET: AcountWalletPaymentApproval
        public ActionResult PaymentApproval(AccountWalletPaymentApprovalRequestVM userInputModel, PagerParameters pagerParameters)
        {
            try
            {
                _accountWalletPaymentApprovalHandler.CheckForPermission(Permissions.CanViewWalletPaymentApprovalReport);

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

                AccountWalletPaymentApprovalSearchParams searchParams = new AccountWalletPaymentApprovalSearchParams
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    Skip = skip,
                    Take = take,
                    PaymentId = userInputModel.PaymentId,
                    SourceAccountName = userInputModel.SourceAccount,
                    UserPartRecordId = _orchardServices.WorkContext.CurrentUser.Id
                };

                var vm = _accountWalletPaymentApprovalHandler.GetPaymentApprovalRequestVM(searchParams);

                var pageShape = Shape.Pager(pager).TotalItemCount(vm.TotalAccountWalletPaymentApprovalRecord);
                pageShape.RouteData(DoRouteDataForAccountWalletPaymentRequestReport(vm));

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

        public ActionResult ViewDetail(string paymentId)
        {
            try
            {
                _accountWalletPaymentApprovalHandler.CheckForPermission(Permissions.CanViewWalletPaymentApprovalReport);
                return View(_accountWalletPaymentApprovalHandler.GetViewDetailVM(paymentId));
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

        public ActionResult Approve(string paymentId)
        {
            try
            {
                Logger.Information($"About to approve a payment request with reference {paymentId}. User Id: {_orchardServices.WorkContext.CurrentUser.Id}");
                _accountWalletPaymentApprovalHandler.CheckForPermission(Permissions.CanViewWalletPaymentApprovalReport);
                string responseMessage = _accountWalletPaymentApprovalHandler.ProcessPaymentRequest(paymentId, approveRequest: true);
                _notifier.Add(NotifyType.Information, PoliceLang.ToLocalizeString($"{responseMessage}"));
                return RedirectToRoute(AccountWalletPaymentApproval.PaymentApproval);
              
            }
            catch (InvalidOperationException)
            {
                _notifier.Add(NotifyType.Error, PoliceLang.ToLocalizeString($"Payment request with paymentId: {paymentId} has already been processed or not authorized"));
                return RedirectToRoute(AccountWalletPaymentApproval.PaymentApproval);
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

        public ActionResult Decline(string paymentId)
        {
            try
            {
                Logger.Information($"About to decline a payment request with reference {paymentId}. User Id: {_orchardServices.WorkContext.CurrentUser.Id}");
                _accountWalletPaymentApprovalHandler.CheckForPermission(Permissions.CanViewWalletPaymentApprovalReport);
                string responseMessage = _accountWalletPaymentApprovalHandler.ProcessPaymentRequest(paymentId, approveRequest: false);
                _notifier.Add(NotifyType.Information, PoliceLang.ToLocalizeString($"{responseMessage}"));
                return RedirectToRoute(AccountWalletPaymentApproval.PaymentApproval);

            }
            catch (InvalidOperationException)
            {
                _notifier.Add(NotifyType.Error, ErrorLang.genericexception());
                return RedirectToRoute(AccountWalletPaymentApproval.PaymentApproval);
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

        private object DoRouteDataForAccountWalletPaymentRequestReport(AccountWalletPaymentApprovalRequestVM model)
        {
            RouteData routeData = new RouteData();
            if (!string.IsNullOrEmpty(model.SourceAccount)) { routeData.Values.Add("SourceAccount", model.SourceAccount); }
            if (!string.IsNullOrEmpty(model.PaymentId)) { routeData.Values.Add("PaymentId", model.PaymentId); }
            return routeData;
        }
    }
}