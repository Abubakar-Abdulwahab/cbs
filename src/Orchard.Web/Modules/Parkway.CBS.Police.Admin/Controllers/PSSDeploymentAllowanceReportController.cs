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
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace Parkway.CBS.Police.Admin.Controllers
{
    [Admin]
    public class PSSDeploymentAllowanceReportController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly INotifier _notifier;
        public ILogger Logger { get; set; }
        dynamic Shape { get; set; }
        private readonly IPSSDeploymentAllowanceHandler _deploymentAllowanceHandler;

        public PSSDeploymentAllowanceReportController(IOrchardServices orchardServices, IPSSDeploymentAllowanceHandler deploymentAllowanceHandler, IShapeFactory shapeFactory)
        {
            _orchardServices = orchardServices;
            _notifier = orchardServices.Notifier;
            Logger = NullLogger.Instance;
            Shape = shapeFactory;
            _deploymentAllowanceHandler = deploymentAllowanceHandler;
        }

        // GET: PSSDeploymentAllowanceReport
        public ActionResult PSSDeploymentAllowanceReport(DeploymentAllowanceReportVM userInput, PagerParameters pagerParameters)
        {
            try
            {
                _deploymentAllowanceHandler.CheckForPermission(Permissions.CanViewDeploymentAllowanceRequests);

                var pager = new Pager(_orchardServices.WorkContext.CurrentSite, pagerParameters);

                DateTime startDate = DateTime.Now.AddMonths(-3);
                DateTime endDate = DateTime.Now;

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

                OfficerDeploymentAllowanceSearchParams searchParams = new OfficerDeploymentAllowanceSearchParams
                {
                    Take = take,
                    Skip = skip,
                    EndDate = endDate,
                    StartDate = startDate,
                    InvoiceNumber = userInput.InvoiceNumber,
                    FileNumber = userInput.FileNumber,
                    AccountNumber = userInput.AccountNumber,
                    IPPISNumber = userInput.IPPISNo,
                    APNumber = userInput.APNumber,
                    RankId = userInput.Rank,
                    RequestStatus = userInput.Status,
                    AdminUserId = _orchardServices.WorkContext.CurrentUser.Id,
                    SelectedCommand = (string.IsNullOrEmpty(userInput.SelectedCommand) && userInput.State > 0) ? "-1" : userInput.SelectedCommand,
                    State = userInput.State,
                    LGA = userInput.LGA
                };

                DeploymentAllowanceReportVM model = _deploymentAllowanceHandler.GetVMForReports(searchParams);
                CommandVM selectedCommand = (model.CommandId > 0) ? model.Commands.Where(x => x.Id == model.CommandId).FirstOrDefault() : null;
                model.SelectedCommandName = (selectedCommand != null) ? selectedCommand.Name : null;
                model.SelectedCommandCode = (selectedCommand != null) ? selectedCommand.Code : null;
                var pageShape = Shape.Pager(pager).TotalItemCount(model.TotalNumberOfDeploymentAllowances);

                pageShape.RouteData(DoRouteDataForRequestsReport(model));
                model.Pager = pageShape;
                model.Status = userInput.Status;

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

        /// <summary>
        /// Get the deployment allowance request detail
        /// </summary>
        /// <param name="deploymentAllowanceRequestId"></param>
        /// <returns>EscortDeploymentRequestDetailsVM</returns>
        public ActionResult RequestDetails(Int64 deploymentAllowanceRequestId)
        {
            try
            {
                _deploymentAllowanceHandler.CheckForPermission(Permissions.CanViewDeploymentAllowanceRequests);

                var details = _deploymentAllowanceHandler.GetDeploymentAllowanceRequestDetails(deploymentAllowanceRequestId);
                details.ShowApprovalForm = false;
                details.ViewName = $"PSSDeploymentAllowanceRequest/{details.ViewName}";
                return View(details.ViewName, details);
            }
            catch (NoRecordFoundException)
            {
                _notifier.Add(NotifyType.Warning, ErrorLang.record404());
                return Redirect("~/Admin");
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


        /// <summary>
        /// Pop route data with available data parameters to help pager keep track of search params
        /// </summary>
        /// <param name="model"></param>
        /// <returns>RouteData</returns>
        private RouteData DoRouteDataForRequestsReport(DeploymentAllowanceReportVM model)
        {
            RouteData routeData = new RouteData();
            if (!string.IsNullOrEmpty(model.InvoiceNumber)) { routeData.Values.Add("invoiceNumber", model.InvoiceNumber); }
            if (!string.IsNullOrEmpty(model.FileNumber)) { routeData.Values.Add("fileNumber", model.FileNumber); }
            if (!string.IsNullOrEmpty(model.AccountNumber)) { routeData.Values.Add("accountNumber", model.AccountNumber); }
            if (!string.IsNullOrEmpty(model.IPPISNo)) { routeData.Values.Add("IPPISNo", model.IPPISNo); }
            if (!string.IsNullOrEmpty(model.APNumber)) { routeData.Values.Add("APNumber", model.APNumber); }
            if (model.Status != DeploymentAllowanceStatus.None) { routeData.Values.Add("status", model.Status); }
            if (model.Rank > 0) { routeData.Values.Add("Rank", model.Rank); }
            if (model.State != 0) { routeData.Values.Add("state", model.State); }
            if (model.LGA != 0) { routeData.Values.Add("lga", model.LGA); }
            if (!string.IsNullOrEmpty(model.SelectedCommand) && model.SelectedCommand != "0") { routeData.Values.Add("SelectedCommand", model.SelectedCommand); }
            routeData.Values.Add("from", model.From);
            routeData.Values.Add("end", model.End);
            return routeData;
        }
    }
}