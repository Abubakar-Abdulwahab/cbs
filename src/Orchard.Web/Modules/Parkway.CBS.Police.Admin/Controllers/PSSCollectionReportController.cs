using Orchard;
using Orchard.DisplayManagement;
using Orchard.Logging;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
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
    public class PSSCollectionReportController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly INotifier _notifier;
        public ILogger Logger { get; set; }
        dynamic Shape { get; set; }

        private readonly IPSSCollectionReportHandler _pssReportHandler;
        private readonly IRequestListHandler _policeRequestHandler;

        public PSSCollectionReportController(IOrchardServices orchardServices, IRequestListHandler policeRequestHandler, IPSSCollectionReportHandler pssReportHandler, IShapeFactory shapeFactory)
        {
            _orchardServices = orchardServices;
            _notifier = orchardServices.Notifier;
            Logger = NullLogger.Instance;
            Shape = shapeFactory;
            _policeRequestHandler = policeRequestHandler;
            _pssReportHandler = pssReportHandler;
        }


        /// <summary>
        /// Get all requests
        /// </summary>
        /// <param name="userInput"></param>
        /// <param name="pagerParameters"></param>
        /// <returns></returns>
        public ActionResult CollectionReport(CollectionReportVM userInput, PagerParameters pagerParameters)
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

                PSSCollectionSearchParams searchParams = new PSSCollectionSearchParams
                {
                    Take = take,
                    Skip = skip,
                    EndDate = endDate,
                    StartDate = startDate,
                    InvoiceNumber = userInput.InvoiceNumber,
                    FileNumber = userInput.FileNumber,
                    PaymentRef = userInput.PaymentRef,
                    ReceiptNumber = userInput.ReceiptNumber,
                    AdminUserId = _orchardServices.WorkContext.CurrentUser.Id,
                    SelectedRevenueHead = userInput.SelectedRevenueHead,
                    State = userInput.State,
                    LGA = userInput.LGA,
                    SelectedCommand = (string.IsNullOrEmpty(userInput.SelectedCommand) && userInput.State > 0) ? "-1" : userInput.SelectedCommand,
                    PaymentDirection = userInput.PaymentDirection == 0 ? CollectionPaymentDirection.PaymentDate : userInput.PaymentDirection
                };

                CollectionReportVM model = _pssReportHandler.GetVMForRequestReport(searchParams);
                var pageShape = Shape.Pager(pager).TotalItemCount(model.TotalRequestRecord);

                pageShape.RouteData(DoRouteDataForRequestsReport(model));
                model.Pager = pageShape;

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
        /// Pop route data with available data parameters to help pager keep track of search params
        /// </summary>
        /// <param name="model"></param>
        /// <returns>RouteData</returns>
        private RouteData DoRouteDataForRequestsReport(CollectionReportVM model)
        {
            RouteData routeData = new RouteData();
            if (!string.IsNullOrEmpty(model.InvoiceNumber)) { routeData.Values.Add("InvoiceNumber", model.InvoiceNumber); }
            if (!string.IsNullOrEmpty(model.FileNumber)) { routeData.Values.Add("FileNumber", model.FileNumber); }
            if (!string.IsNullOrEmpty(model.PaymentRef)) { routeData.Values.Add("PaymentRef", model.PaymentRef); }
            if (!string.IsNullOrEmpty(model.ReceiptNumber)) { routeData.Values.Add("ReceiptNumber", model.ReceiptNumber); }
            if (!string.IsNullOrEmpty(model.SelectedRevenueHead) && model.SelectedRevenueHead != "0") { routeData.Values.Add("SelectedRevenueHead", model.SelectedRevenueHead); }
            if (!string.IsNullOrEmpty(model.SelectedCommand) && model.SelectedCommand != "0") { routeData.Values.Add("SelectedCommand", model.SelectedCommand); }
            if (model.State != 0) { routeData.Values.Add("State", model.State); }
            if (model.LGA != 0) { routeData.Values.Add("LGA", model.LGA); }
            routeData.Values.Add("from", model.From);
            routeData.Values.Add("end", model.End);
            return routeData;
        }

    }
}