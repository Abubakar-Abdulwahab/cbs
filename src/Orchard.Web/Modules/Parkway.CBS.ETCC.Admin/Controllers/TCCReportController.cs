using Orchard;
using Orchard.Logging;
using Orchard.UI.Admin;
using Orchard.UI.Notify;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Lang;
using System;
using System.Web.Mvc;
using Orchard.DisplayManagement;
using System.Web.Routing;
using Orchard.UI.Navigation;
using Parkway.CBS.ETCC.Admin.VM;
using Parkway.CBS.ETCC.Admin.Controllers.Handlers.Contracts;
using System.Globalization;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models.Enums;

namespace Parkway.CBS.ETCC.Admin.Controllers
{
    public class TCCReportController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly INotifier _notifier;
        public ILogger Logger { get; set; }
        dynamic Shape { get; set; }
        private readonly ITCCReportHandler _tccReportHandler;

        public TCCReportController(IOrchardServices orchardServices, IShapeFactory shapeFactory, ITCCReportHandler tccReportHandler)
        {
            _orchardServices = orchardServices;
            _notifier = orchardServices.Notifier;
            Logger = NullLogger.Instance;
            Shape = shapeFactory;
            _tccReportHandler = tccReportHandler;
        }

        [Admin]
        /// <summary>
        /// Get TCC request based on user selected filters
        /// </summary>
        /// <param name="userInput"></param>
        /// <param name="pagerParameters"></param>
        /// <returns>TCCRequestReportVM</returns>
        public ActionResult RequestReport(TCCRequestReportVM userInput, PagerParameters pagerParameters)
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

                TCCReportSearchParams searchParams = new TCCReportSearchParams
                {
                    Take = take,
                    Skip = skip,
                    EndDate = endDate,
                    StartDate = startDate,
                    ApplicationNumber = userInput.ApplicationNumber,
                    ApplicantName = userInput.ApplicantName,
                    PayerId = userInput.PayerId,
                    SelectedStatus = userInput.Status,
                };

                TCCRequestReportVM model = _tccReportHandler.GetRequestReport(searchParams);
                var pageShape = Shape.Pager(pager).TotalItemCount(model.TotalRequestRecord);

                pageShape.RouteData(DoRouteDataForRequestsReport(model));
                model.Pager = pageShape;

                return View(model);
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

        [Admin]
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
            if (!string.IsNullOrEmpty(model.PayerId)) { routeData.Values.Add("PayerId", model.PayerId); }
            if (!string.IsNullOrEmpty(model.ApplicantName)) { routeData.Values.Add("ApplicantName", model.ApplicantName); }
            if (!string.IsNullOrEmpty(model.ApplicationNumber)) { routeData.Values.Add("ApplicationNumber", model.ApplicationNumber); }
            if (model.Status != TCCRequestStatus.None) { routeData.Values.Add("status", model.Status); }
            routeData.Values.Add("from", model.From);
            routeData.Values.Add("end", model.End);
            return routeData;
        }


        /// <summary>
        /// View receipt
        /// </summary>
        /// <param name="tccNumber"></param>
        /// <returns></returns>
        public ActionResult ViewCertificate(string tccNumber)
        {
            try
            {
                CreateReceiptDocumentVM result = _tccReportHandler.CreateCertificateByteFile(tccNumber);
                return File(result.DocByte, "application/pdf");
            }
            catch (UserNotAuthorizedForThisActionException exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Information, ErrorLang.usernotauthorized());
            }
            catch (NoRecordFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Information, ErrorLang.norecord404());
            }
            catch (Exception ex)
            {
                _notifier.Error(ErrorLang.genericexception());
                Logger.Error(ex, ex.Message);
            }
            return Redirect("~/Admin");
        }

    }
}