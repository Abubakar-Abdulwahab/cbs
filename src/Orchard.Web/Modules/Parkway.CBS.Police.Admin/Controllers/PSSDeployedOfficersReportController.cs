using System;
using Orchard;
using System.Web.Mvc;
using Orchard.Logging;
using Orchard.UI.Admin;
using Orchard.UI.Notify;
using Parkway.CBS.Core.Lang;
using Orchard.DisplayManagement;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Orchard.UI.Navigation;
using Parkway.CBS.Police.Admin.VM;
using System.Globalization;
using System.Web.Routing;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Police.Core.VM;
using Parkway.CBS.Core.Models.Enums;
using Newtonsoft.Json;
using Parkway.CBS.Police.Core.HelperModels;
using System.Linq;
using Parkway.CBS.Police.Core.Lang;

namespace Parkway.CBS.Police.Admin.Controllers
{
    [Admin]
    public class PSSDeployedOfficersReportController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly INotifier _notifier;
        public ILogger Logger { get; set; }
        dynamic Shape { get; set; }

        private readonly IDeploymentReportHandler _deploymentReportHandler;

        public PSSDeployedOfficersReportController(IOrchardServices orchardServices, IDeploymentReportHandler deploymentReportHandler, IShapeFactory shapeFactory)
        {
            _orchardServices = orchardServices;
            _notifier = orchardServices.Notifier;
            Logger = NullLogger.Instance;
            Shape = shapeFactory;
            _deploymentReportHandler = deploymentReportHandler;
        }


        public ActionResult PSSDeployedOfficers(DeploymentReportVM userInput, PagerParameters pagerParameters)
        {
            try
            {
                Logger.Error("PSSDeployedOfficersReportController");
                Logger.Error(JsonConvert.SerializeObject(userInput));
                _deploymentReportHandler.CheckForPermission(Permissions.CanViewDeployments);

                var pager = new Pager(_orchardServices.WorkContext.CurrentSite, pagerParameters);

                DateTime startDate = DateTime.Now.Date.AddMonths(-3);
                DateTime endDate = DateTime.Now.Date;

                if (!string.IsNullOrEmpty(userInput.From) && !string.IsNullOrEmpty(userInput.End))
                {
                    try
                    {
                        startDate = DateTime.ParseExact(userInput.From, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        endDate = DateTime.ParseExact(userInput.End, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
                //get the date up until the final sec
                endDate = endDate.Date.AddDays(1).AddMilliseconds(-1);

                int take = pagerParameters.PageSize.HasValue ? pagerParameters.PageSize.Value : 10;

                if (take == 0) { take = 10; pager.PageSize = 10; }
                int skip = pagerParameters.Page.HasValue ? (pagerParameters.Page.Value - 1) * take : 0;
              
                OfficerDeploymentSearchParams searchParams = new OfficerDeploymentSearchParams
                {
                    Take = take,
                    Skip = skip,
                    EndDate = endDate,
                    StartDate = startDate,
                    AdminUserId = _orchardServices.WorkContext.CurrentUser.Id,
                    SelectedCommand = (string.IsNullOrEmpty(userInput.SelectedCommand) && userInput.State > 0) ? "-1" : userInput.SelectedCommand,
                    CustomerName = userInput.CustomerName,
                    InvoiceNumber = userInput.InvoiceNumber,
                    IPPISNo = userInput.IPPISNo,
                    APNumber = userInput.APNumber,
                    RequestRef = userInput.RequestRef,
                    Rank = userInput.Rank,
                    LGA = userInput.LGA,
                    State = userInput.State,
                    OfficerName = userInput.OfficerName,
                };

                DeploymentReportVM vm = _deploymentReportHandler.GetVMForReports(searchParams);
                CommandVM selectedCommand = (vm.CommandId > 0) ? vm.Commands.Where(x => x.Id == vm.CommandId).FirstOrDefault() : null;
                vm.SelectedCommandName = (selectedCommand != null) ? selectedCommand.Name : null;
                vm.SelectedCommandCode = (selectedCommand != null) ? selectedCommand.Code : null;
                var pageShape = Shape.Pager(pager).TotalItemCount(vm.TotalNumberOfDeployments);

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



        private object DoRouteDataForDeploymentReport(DeploymentReportVM model)
        {
            RouteData routeData = new RouteData();
            if (!string.IsNullOrEmpty(model.OfficerName)) { routeData.Values.Add("officerName", model.OfficerName); }
            if (!string.IsNullOrEmpty(model.CustomerName)) { routeData.Values.Add("customerName", model.CustomerName); }
            if (!string.IsNullOrEmpty(model.InvoiceNumber)) { routeData.Values.Add("invoiceNumber", model.InvoiceNumber); }
            if (model.State != 0) { routeData.Values.Add("state", model.State); }
            if (model.LGA != 0) { routeData.Values.Add("lga", model.LGA); }
            if (!string.IsNullOrEmpty(model.SelectedCommand) && model.SelectedCommand != "0") { routeData.Values.Add("SelectedCommand", model.SelectedCommand); }
            routeData.Values.Add("from", model.From);
            routeData.Values.Add("end", model.End);
            return routeData;
        }


    }
}