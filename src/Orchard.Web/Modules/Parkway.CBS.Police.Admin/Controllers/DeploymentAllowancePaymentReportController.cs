using Orchard;
using Orchard.DisplayManagement;
using Orchard.Logging;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Lang;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Routing;

namespace Parkway.CBS.Police.Admin.Controllers
{
    [Admin]
    public class DeploymentAllowancePaymentReportController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly INotifier _notifier;
        private readonly IDeploymentAllowancePaymentReportHandler _handler;
        public ILogger Logger { get; set; }
        dynamic Shape { get; set; }

        public DeploymentAllowancePaymentReportController(IShapeFactory shapeFactory, IOrchardServices orchardServices, IDeploymentAllowancePaymentReportHandler handler)
        {
            _orchardServices = orchardServices;
            _notifier = orchardServices.Notifier;
            _handler = handler;
            Logger = NullLogger.Instance;
            Shape = shapeFactory;
        }


        public ActionResult Report(DeploymentAllowancePaymentReportVM userInputModel, PagerParameters pagerParameters)
        {
            try
            {
                _handler.CheckForPermission(Permissions.CanViewDeploymentAllowancePaymentReport);

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

                DeploymentAllowancePaymentSearchParams searchParams = new DeploymentAllowancePaymentSearchParams
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    Skip = skip,
                    UserPartRecordId = _orchardServices.WorkContext.CurrentUser.Id,
                    Take = take,
                    PaymentRef = userInputModel.PaymentRef?.Trim(),
                    SourceAccountName = userInputModel.SourceAccount?.Trim(),
                    Status = userInputModel.Status,
                    CommandTypeId = userInputModel.CommandTypeId
                };

                var vm = _handler.GetDeploymentAllowancePaymentReportVM(searchParams);

                var pageShape = Shape.Pager(pager).TotalItemCount(vm.TotalDeploymentAllowancePaymentReportRecord);
                pageShape.RouteData(DoRouteDataForDeploymentAllowancePaymentReport(vm));

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
                _notifier.Add(NotifyType.Error, PoliceErrorLang.genericexception());
                return Redirect("~/Admin");
            }
        }

        private object DoRouteDataForDeploymentAllowancePaymentReport(DeploymentAllowancePaymentReportVM model)
        {
            RouteData routeData = new RouteData();
            if (!string.IsNullOrEmpty(model.SourceAccount)) { routeData.Values.Add("SourceAccount", model.SourceAccount); }
            if (!string.IsNullOrEmpty(model.PaymentRef)) { routeData.Values.Add("PaymentRef", model.PaymentRef); }
            if (model.Status > 0) { routeData.Values.Add("Status", model.Status); }
            if (model.CommandTypeId > 0) { routeData.Values.Add("CommandTypeId", model.CommandTypeId); }
            return routeData;
        }
    }
}