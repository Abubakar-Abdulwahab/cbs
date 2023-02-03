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
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Lang;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace Parkway.CBS.Police.Admin.Controllers
{
    [Admin]
    public class PoliceOfficerReportController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly INotifier _notifier;
        public ILogger Logger { get; set; }
        dynamic Shape { get; set; }
        private readonly IPoliceOfficerReportHandler _policeOfficerReportHandler;


        public PoliceOfficerReportController(IOrchardServices orchardServices, INotifier notifier, IPoliceOfficerReportHandler policeOfficerReportHandler, IShapeFactory shapeFactory)
        {
            _orchardServices = orchardServices;
            _notifier = notifier;
            _policeOfficerReportHandler = policeOfficerReportHandler;
            Logger = NullLogger.Instance;
            Shape = shapeFactory;
        }

        /// <summary>
        /// Get a report of all police officers
        /// </summary>
        /// <param name="userInput"></param>
        /// <param name="pagerParameters"></param>
        /// <returns></returns>
        public ActionResult PoliceOfficerReport(PoliceOfficerReportVM userInput, PagerParameters pagerParameters)
        {
            try
            {
                _policeOfficerReportHandler.CheckForPermission(Permissions.CanViewOfficers);

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

                PoliceOfficerReportVM vm = _policeOfficerReportHandler.GetVMForReports(searchParams);

                CommandVM selectedCommand= (vm.CommandId > 0) ? vm.Commands.Where(x => x.Id == vm.CommandId).FirstOrDefault() : null;
                vm.SelectedCommandName = (selectedCommand != null) ? selectedCommand.Name : null;
                vm.SelectedCommandCode = (selectedCommand != null) ? selectedCommand.Code : null;
                var pageShape = Shape.Pager(pager).TotalItemCount(vm.TotalActiveOfficersRecord);

                pageShape.RouteData(DoRouteDataForDeploymentReport(vm));

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