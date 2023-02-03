using Orchard;
using Orchard.Logging;
using Orchard.UI.Admin;
using Orchard.UI.Notify;
using Parkway.CBS.POSSAP.Scheduler.Controllers.Handlers.Contracts;
using System.Web.Mvc;
using Orchard.DisplayManagement;
using Orchard.UI.Navigation;
using Parkway.CBS.Police.Admin.VM;
using Parkway.CBS.Police.Core.VM;
using Parkway.CBS.Police.Core.HelperModels;
using System;
using System.Linq;
using System.Web.Routing;

namespace Parkway.CBS.POSSAP.Scheduler.Controllers
{
    [Admin]
    public class PoliceOfficerSchedulerReportController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly INotifier _notifier;
        public ILogger Logger { get; set; }
        dynamic Shape { get; set; }
        private readonly IPoliceOfficerReportSchedulerHandler _handler;

        public PoliceOfficerSchedulerReportController(IOrchardServices orchardServices, INotifier notifier, IPoliceOfficerReportSchedulerHandler handler, IShapeFactory shapeFactory)
        {
            _orchardServices = orchardServices;
            _notifier = notifier;
            _handler = handler;
            Logger = NullLogger.Instance;
            Shape = shapeFactory;
        }


        // GET: PoliceOfficerReport
        public ActionResult Report(PoliceOfficerReportVM userInput, PagerParameters pagerParameters)
        {
            _handler.CheckForPermission(Permissions.CanViewOfficersSchedule);

            var pager = new Pager(_orchardServices.WorkContext.CurrentSite, pagerParameters);

            int take = pagerParameters.PageSize.HasValue ? pagerParameters.PageSize.Value : 10;

            if (take == 0) { take = 10; pager.PageSize = 10; }
            int skip = pagerParameters.Page.HasValue ? (pagerParameters.Page.Value - 1) * take : 0;

            PoliceOfficerSearchParams searchParams = new PoliceOfficerSearchParams
            {
                Take = take,
                Skip = skip,
                AdminUserId = _orchardServices.WorkContext.CurrentUser.Id,
                SelectedCommand = (string.IsNullOrEmpty(userInput.SelectedCommand) && userInput.SelectedState > 0) ? "-1" : userInput.SelectedCommand,
                IPPISNo = userInput.IppisNumber,
                Rank = userInput.SelectedRank,
                LGA = userInput.SelectedLGA,
                State = userInput.SelectedState,
                OfficerName = userInput.OfficerName,
                IdNumber = userInput.IdNumber
            };

            PoliceOfficerReportVM vm = _handler.GetVMForReports(searchParams);
            //.GetVMForReports(searchParams);

            CommandVM selectedCommand = (vm.CommandId > 0) ? vm.Commands.Where(x => x.Id == vm.CommandId).FirstOrDefault() : null;
            vm.SelectedCommandName = (selectedCommand != null) ? selectedCommand.Name : null;
            vm.SelectedCommandCode = (selectedCommand != null) ? selectedCommand.Code : null;
            var pageShape = Shape.Pager(pager).TotalItemCount(vm.TotalActiveOfficersRecord);

            pageShape.RouteData(DoRouteDataForDeploymentReport(vm));

            vm.Pager = pageShape;

            return View(vm);
        }


        private object DoRouteDataForDeploymentReport(PoliceOfficerReportVM model)
        {
            RouteData routeData = new RouteData();
            if (!string.IsNullOrEmpty(model.OfficerName)) { routeData.Values.Add("OfficerName", model.OfficerName); }
            if (!string.IsNullOrEmpty(model.IppisNumber)) { routeData.Values.Add("IppisNumber", model.IppisNumber); }
            if (!string.IsNullOrEmpty(model.IdNumber)) { routeData.Values.Add("IdNumber", model.IdNumber); }
            if (!string.IsNullOrEmpty(model.SelectedCommand) && model.SelectedCommand != "0") { routeData.Values.Add("SelectedCommand", model.SelectedCommand); }
            if (model.SelectedState > 0) { routeData.Values.Add("SelectedState", model.SelectedState); }
            if (model.SelectedLGA > 0) { routeData.Values.Add("SelectedLGA", model.SelectedLGA); }
            return routeData;
        }

    }
}