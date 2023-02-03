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
    [Admin]
    public class ReceiptUtilizationReportController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly INotifier _notifier;
        public ILogger Logger { get; set; }
        dynamic Shape { get; set; }
        private readonly IPAYEReceiptUtilizationHandler _payeReceiptUtilizationHandler;

        public ReceiptUtilizationReportController(IOrchardServices orchardServices, IShapeFactory shapeFactory, IPAYEReceiptUtilizationHandler payeReceiptUtilizationHandler)
        {
            _orchardServices = orchardServices;
            _notifier = orchardServices.Notifier;
            Logger = NullLogger.Instance;
            Shape = shapeFactory;
            _payeReceiptUtilizationHandler = payeReceiptUtilizationHandler;
        }


        /// <summary>
        /// Route name: tcc.admin.paye.receipts.report
        /// Path: Admin/TCC/PAYE/Receipts/Report
        /// Get PAYE receipts list using the user selected filters
        /// </summary>
        /// <param name="userInput"></param>
        /// <param name="pagerParameters"></param>
        public virtual ActionResult Receipts(PAYEReceiptReportVM userInput, PagerParameters pagerParameters)
        {
            try
            {
                _payeReceiptUtilizationHandler.CheckForPermission(Permissions.CanViewReceiptUtilizations);
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

                PAYEReceiptSearchParams searchData = new PAYEReceiptSearchParams
                {
                    ReceiptNumber = userInput.ReceiptNumber?.Trim().ToUpper(),
                    Take = take,
                    Skip = skip,
                    EndDate = endDate,
                    StartDate = startDate,
                    PayerId = userInput.PayerId,
                    Status = userInput.Status
                };

                PAYEReceiptReportVM model = _payeReceiptUtilizationHandler.GetPAYEReceipts(searchData);
                var pageShape = Shape.Pager(pager).TotalItemCount(model.TotalRequestRecord);

                pageShape.RouteData(DoRouteDataForRequestsReport(model));
                model.Pager = pageShape;

                return View(model);
            }
            catch (Exception exception) { Logger.Error(exception, exception.Message); }
            TempData = null;
            TempData.Add("Error", ErrorLang.genericexception().ToString());
            return Redirect("~/Admin");
        }

        /// <summary>
        /// Route name: tcc.admin.paye.receipts.utilizations
        /// Path: Admin/TCC/PAYE/Receipts/utilizations/{receiptNumber}
        /// Get utilizations for PAYE receipt using the specified receipt number
        /// </summary>
        /// <param name="receiptNumber"></param>
        /// <returns></returns>
        public ActionResult Utilizations(string receiptNumber)
        {
            try
            {
                _payeReceiptUtilizationHandler.CheckForPermission(Permissions.CanViewReceiptUtilizations);
                if (string.IsNullOrEmpty(receiptNumber)) { throw new Exception("Receipt number not specified."); }
                return View(_payeReceiptUtilizationHandler.GetUtilizationsReport(receiptNumber.Trim()));
            }
            catch (Exception exception) { Logger.Error(exception, exception.Message); }
            TempData = null;
            TempData.Add("Error", ErrorLang.genericexception().ToString());
            return Redirect("~/Admin");
        }

        /// <summary>
        /// Pop route data with available data parameters to help pager keep track of search params
        /// </summary>
        /// <param name="model"></param>
        /// <returns>RouteData</returns>
        private RouteData DoRouteDataForRequestsReport(PAYEReceiptReportVM model)
        {
            RouteData routeData = new RouteData();
            if (!string.IsNullOrEmpty(model.PayerId)) { routeData.Values.Add("PayerId", model.PayerId); }
            if (!string.IsNullOrEmpty(model.ReceiptNumber)) { routeData.Values.Add("ReceiptNumber", model.ReceiptNumber); }
            if (model.Status != PAYEReceiptUtilizationStatus.None) { routeData.Values.Add("status", model.Status); }
            routeData.Values.Add("from", model.From);
            routeData.Values.Add("end", model.End);
            return routeData;
        }

    }
}