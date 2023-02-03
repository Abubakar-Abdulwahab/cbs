using Orchard;
using Orchard.Logging;
using Orchard.UI.Admin;
using Orchard.UI.Notify;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Lang;
using Orchard.UI.Navigation;
using System;
using System.Web.Mvc;
using System.Globalization;
using Orchard.DisplayManagement;
using System.Web.Routing;
using Parkway.CBS.ETCC.Admin.VM;
using Parkway.CBS.ETCC.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models.Enums;
using System.Collections.Generic;
using Parkway.CBS.ETCC.Admin.RouteName;
using System.Linq;

namespace Parkway.CBS.ETCC.Admin.Controllers
{
    [Admin]
    public class RequestApprovalController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly INotifier _notifier;
        public ILogger Logger { get; set; }
        dynamic Shape { get; set; }
        private readonly ITCCReportHandler _tccReportHandler;
        private readonly IRequestApprovalHandler _requestApprovalHandler;

        public RequestApprovalController(IOrchardServices orchardServices, IShapeFactory shapeFactory, ITCCReportHandler tccReportHandler, IRequestApprovalHandler requestApprovalHandler)
        {
            _orchardServices = orchardServices;
            _notifier = orchardServices.Notifier;
            Logger = NullLogger.Instance;
            Shape = shapeFactory;
            _tccReportHandler = tccReportHandler;
            _requestApprovalHandler = requestApprovalHandler;
        }

        /// <summary>
        /// Get TCC request based on user selected filters
        /// </summary>
        /// <param name="userInput"></param>
        /// <param name="pagerParameters"></param>
        /// <returns>TCCRequestReportVM</returns>
        public ActionResult TCCRequests(TCCRequestReportVM userInput, PagerParameters pagerParameters)
        {
            try
            {
                _tccReportHandler.CheckForPermission(Permissions.CanViewTCCRequests);
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

                int adminUserId = _orchardServices.WorkContext.CurrentUser.Id;
                WorkflowApproverDetailVM approverDetails = _tccReportHandler.GetApproverDetails(adminUserId);

                TCCReportSearchParams searchParams = new TCCReportSearchParams
                {
                    Take = take,
                    Skip = skip,
                    EndDate = endDate,
                    StartDate = startDate,
                    ApplicationNumber = userInput.ApplicationNumber,
                    ApplicantName = userInput.ApplicantName,
                    PayerId = userInput.PayerId,
                    SelectedStatus = TCCRequestStatus.PendingApproval,
                    ApprovalLevelId = approverDetails == null? 0 : approverDetails.ApprovalLevelId
                };

                TCCRequestReportVM model = _tccReportHandler.GetRequestReport(searchParams);
                var pageShape = Shape.Pager(pager).TotalItemCount(model.TotalRequestRecord);

                pageShape.RouteData(DoRouteDataForRequestsReport(model));
                model.Pager = pageShape;

                return View(model);
            }
            catch (NoRecordFoundException)
            {
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotauthorized());
                return Redirect("~/Admin");
            }
            catch (UserNotAuthorizedForThisActionException)
            {
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotauthorized());
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
        /// Get TCC request details for a particular application number
        /// </summary>
        /// <param name="applicationNumber"></param>
        /// <returns>TCCRequestDetailVM</returns>
        public ActionResult TCCRequestDetails(string applicationNumber)
        {
            try
            {
                _tccReportHandler.CheckForPermission(Permissions.CanViewTCCRequests);

                var details = _tccReportHandler.GetRequestDetail(applicationNumber);

                return View(details);
            }
            catch (NoRecordFoundException)
            {
                _notifier.Add(NotifyType.Warning, ErrorLang.record404());
                return Redirect("~/Admin");
            }
            catch (UserNotAuthorizedForThisActionException)
            {
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotauthorized());
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
        private RouteData DoRouteDataForRequestsReport(TCCRequestReportVM model)
        {
            RouteData routeData = new RouteData();
            if (!string.IsNullOrEmpty(model.PayerId)) { routeData.Values.Add("TIN", model.PayerId); }
            if (!string.IsNullOrEmpty(model.ApplicantName)) { routeData.Values.Add("ApplicantName", model.ApplicantName); }
            if (!string.IsNullOrEmpty(model.ApplicationNumber)) { routeData.Values.Add("ApplicationNumber", model.ApplicationNumber); }
            routeData.Values.Add("from", model.From);
            routeData.Values.Add("end", model.End);
            return routeData;
        }

        /// <summary>
        /// Approve TCC request
        /// </summary>
        /// <param name="approvalDetails"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ApproveRequest(TCCRequestDetailVM approvalDetails)
        {
            List<ErrorModel> errors = new List<ErrorModel> { };
            try
            {
                _requestApprovalHandler.CheckForPermission(Permissions.CanApproveTCCRequests);
                var response = _requestApprovalHandler.ProcessRequestApproval(approvalDetails, ref errors);
                string notificationMsg = string.Format("Request approved successfully.");
                _notifier.Add(NotifyType.Information, Lang.pssrequestapprovedsuccessfully(notificationMsg));

                return RedirectToRoute(RequestApproval.TCCRequestApprovalList);
            }
            catch (DirtyFormDataException)
            {
                _notifier.Add(NotifyType.Error, ErrorLang.ToLocalizeString(errors.First().ErrorMessage));
                return RedirectToRoute(RequestApproval.TCCRequestApprovalDetails, new { applicationNumber = approvalDetails.ApplicationNumber });
            }
            catch (NoRecordFoundException ex)
            {
                _notifier.Add(NotifyType.Error, ErrorLang.norecord404());
                return Redirect("~/Admin");
            }
            catch (UserNotAuthorizedForThisActionException)
            {
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotauthorized());
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
        /// Reject TCC request
        /// </summary>
        /// <param name="approvalDetails"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RejectRequest(TCCRequestDetailVM approvalDetails)
        {
            List<ErrorModel> errors = new List<ErrorModel> { };
            try
            {
                _requestApprovalHandler.CheckForPermission(Permissions.CanApproveTCCRequests);
                var response = _requestApprovalHandler.ProcessRequestRejection(approvalDetails, ref errors);
                string notificationMsg = string.Format("Request rejected successfully.");
                _notifier.Add(NotifyType.Information, Lang.pssrequestrejectedsuccessfully(notificationMsg));

                return RedirectToRoute(RequestApproval.TCCRequestApprovalList);
            }
            catch (DirtyFormDataException)
            {
                _notifier.Add(NotifyType.Error, ErrorLang.ToLocalizeString(errors.First().ErrorMessage));
                return RedirectToRoute(RequestApproval.TCCRequestApprovalDetails, new { applicationNumber = approvalDetails.ApplicationNumber });
            }
            catch (NoRecordFoundException ex)
            {
                _notifier.Add(NotifyType.Error, ErrorLang.norecord404());
                return Redirect("~/Admin");
            }
            catch (UserNotAuthorizedForThisActionException)
            {
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotauthorized());
                return Redirect("~/Admin");
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.genericexception());
                return Redirect("~/Admin");
            }
        }

    }
}