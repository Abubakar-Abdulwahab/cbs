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

    public class PSSExpenditureHeadReportController : Controller
    {
        private readonly IOrchardServices _orchardServices;

        private readonly INotifier _notifier;
        public ILogger Logger { get; set; }
        dynamic Shape { get; set; }

        private readonly IExpenditureHeadReportHandler _expenditureHeadReportHandler;

        public PSSExpenditureHeadReportController(IOrchardServices orchardServices, INotifier notifier, IExpenditureHeadReportHandler expenditureHeadReportHandler, IShapeFactory shapeFactory)
        {
            _orchardServices = orchardServices;
            _notifier = notifier;
            Logger = NullLogger.Instance;
            _expenditureHeadReportHandler = expenditureHeadReportHandler;
            Shape = shapeFactory;
        }

        // GET: PSSExpenditureHeadReport
        /// <summary>
        /// Get a report of all expenditure head
        /// </summary>
        /// <param name="pagerParameters"></param>
        /// <returns></returns>
        public ActionResult Report(ExpenditureHeadReportVM userInputModel, PagerParameters pagerParameters)
        {
            try
            {
                _expenditureHeadReportHandler.CheckForPermission(Permissions.CanViewExpenditureHead);

                var pager = new Pager(_orchardServices.WorkContext.CurrentSite, pagerParameters);

                int take = pagerParameters.PageSize ?? 10;

                if (take == 0) { take = 10; pager.PageSize = 10; }
                int skip = pagerParameters.Page.HasValue ? (pagerParameters.Page.Value - 1) * take : 0;

                ExpenditureHeadReportSearchParams searchParams = new ExpenditureHeadReportSearchParams
                {
                    Take = take,
                    Skip = skip,
                    ExpenditureHeadName = userInputModel.ExpenditureHeadName?.Trim(),
                    Code = userInputModel.Code?.Trim(),
                    Status = userInputModel.Status
                };

                ExpenditureHeadReportVM vm = _expenditureHeadReportHandler.GetVMForReports(searchParams);

                var pageShape = Shape.Pager(pager).TotalItemCount(vm.TotalExpenditureHeadRecord);
                pageShape.RouteData(DoRouteDataForExpenditureHeadReport(vm));

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

        private object DoRouteDataForExpenditureHeadReport(ExpenditureHeadReportVM model)
        {
            RouteData routeData = new RouteData();
            if (!string.IsNullOrEmpty(model.ExpenditureHeadName)) { routeData.Values.Add("ExpenditureHeadName", model.ExpenditureHeadName); }
            if (!string.IsNullOrEmpty(model.Code)) { routeData.Values.Add("Code", model.Code); }
            if (model.Status != (int)Core.Models.Enums.StatusFilter.All) { routeData.Values.Add("Status", model.Status); }
            return routeData;
        }
    }
}