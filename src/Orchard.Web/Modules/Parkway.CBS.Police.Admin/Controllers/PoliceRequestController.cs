using Orchard;
using Orchard.Logging;
using Orchard.UI.Admin;
using Orchard.UI.Notify;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Orchard.UI.Navigation;
using Parkway.CBS.Police.Admin.VM;
using System;
using System.Web.Mvc;
using System.Globalization;
using Parkway.CBS.Police.Core.VM;
using Orchard.DisplayManagement;
using System.Web.Routing;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Lang;

namespace Parkway.CBS.Police.Admin.Controllers
{
    [Admin]
    public class PoliceRequestController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly INotifier _notifier;
        public ILogger Logger { get; set; }
        dynamic Shape { get; set; }

        private readonly IRequestApprovalHandler _requestApprovalHandler;
        private readonly IRequestListHandler _policeRequestHandler;


        public PoliceRequestController(IOrchardServices orchardServices, IRequestListHandler policeRequestHandler, IRequestApprovalHandler requestApprovalHandler, IShapeFactory shapeFactory)
        {
            _orchardServices = orchardServices;
            _notifier = orchardServices.Notifier;
            Logger = NullLogger.Instance;
            Shape = shapeFactory;
            _policeRequestHandler = policeRequestHandler;
            _requestApprovalHandler = requestApprovalHandler;
        }


        /// <summary>
        /// Get all requests
        /// </summary>
        /// <param name="userInput"></param>
        /// <param name="pagerParameters"></param>
        public ActionResult PSSRequests(RequestReportVM userInput, PagerParameters pagerParameters)
        {
            try
            {
                _policeRequestHandler.CheckForPermission(Permissions.CanViewRequests);

                var pager = new Pager(_orchardServices.WorkContext.CurrentSite, pagerParameters);

                DateTime startDate = DateTime.Now.Date.AddMonths(-3);
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

                RequestsReportSearchParams searchParams = new RequestsReportSearchParams
                {
                    Take = take,
                    Skip = skip,
                    EndDate = endDate,
                    StartDate = startDate,
                    RequestOptions = new RequestOptions { InvoiceNumber = userInput.InvoiceNumber, FileNumber = userInput.FileNumber, RequestStatus = userInput.Status, ApprovalNumber = userInput.ApprovalNumber },
                    AdminUserId = _orchardServices.WorkContext.CurrentUser.Id,
                    State = userInput.State,
                    LGA = userInput.LGA,
                    SelectedCommand = (string.IsNullOrEmpty(userInput.SelectedCommand) && userInput.State > 0) ? "-1" : userInput.SelectedCommand,
                    SelectedServiceId = userInput.ServiceType,
                    OrderByColumnName = nameof(Core.DTO.PSSRequestVM.RequestDate)
                };

                RequestReportVM model = _policeRequestHandler.GetVMForRequestReport(searchParams);
                var pageShape = Shape.Pager(pager).TotalItemCount(model.TotalRequestRecord);

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
        /// Get escort or extract request view details
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="serviceTypeId"></param>
        /// <returns></returns>
        public ActionResult PSSRequestDetails(Int64 requestId)
        {
            try
            {
                _policeRequestHandler.CheckForPermission(Permissions.CanViewRequests);
                PSSRequestDetailsVM details = _requestApprovalHandler.GetServiceRequestViewDetailsForView(requestId);
                return View(details);
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
        private RouteData DoRouteDataForRequestsReport(RequestReportVM model)
        {
            RouteData routeData = new RouteData();
            if (!string.IsNullOrEmpty(model.InvoiceNumber)) { routeData.Values.Add("invoiceNumber", model.InvoiceNumber); }
            if (!string.IsNullOrEmpty(model.FileNumber)) { routeData.Values.Add("fileNumber", model.FileNumber); }
            if (!string.IsNullOrEmpty(model.ServiceType) && model.ServiceType != "0") { routeData.Values.Add("serviceType", model.ServiceType); }
            if (model.Status != Core.Models.Enums.PSSRequestStatus.None) { routeData.Values.Add("status", model.Status); }
            if (!string.IsNullOrEmpty(model.SelectedCommand) && model.SelectedCommand != "0") { routeData.Values.Add("SelectedCommand", model.SelectedCommand); }
            if (model.State != 0) { routeData.Values.Add("State", model.State); }
            if (model.LGA != 0) { routeData.Values.Add("LGA", model.LGA); }
            routeData.Values.Add("from", model.From);
            routeData.Values.Add("end", model.End);
            return routeData;
        }


    }
}