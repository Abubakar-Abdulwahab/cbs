using Orchard.Logging;
using Orchard.Themes;
using Parkway.CBS.Client.Web.Controllers.Handlers.Contracts;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Module.Web.Middleware.Filters;
using System;
using System.Globalization;
using System.Web.Mvc;

namespace Parkway.CBS.Client.Web.Controllers
{
    [Themed]
    public class PAYEReceiptUtilizationReportController : Controller
    {
        public ILogger Logger { get; set; }
        private readonly ICommonHandler _commonHandler;
        private readonly IPAYEReceiptUtilizationHandler _payeReceiptUtilizationHandler;

        public PAYEReceiptUtilizationReportController(ICommonHandler commonHandler, IPAYEReceiptUtilizationHandler payeReceiptUtilizationHandler)
        {
            _commonHandler = commonHandler;
            _payeReceiptUtilizationHandler = payeReceiptUtilizationHandler;
            Logger = NullLogger.Instance;
        }

        /// <summary>
        /// Route name: C.PAYE.Receipts
        /// Path: c/paye/receipts
        /// Get PAYE receipts list using the user selected filters
        /// </summary>
        /// <param name="datefilter"></param>
        /// <param name="receiptNumber"></param>
        [BrowserHeaderFilter]
        public virtual ActionResult Receipts(string datefilter, string receiptNumber)
        {
            try
            {
                var user = _commonHandler.GetLoggedInUserDetails();
                if (user == null || user.Entity == null) { TempData.Add("Error", ErrorLang.requiressignin().ToString()); return RedirectToAction("SignIn"); }

                DateTime startDate = DateTime.Now.AddMonths(-3);
                DateTime endDate = DateTime.Now;
                if (!string.IsNullOrEmpty(datefilter))
                {
                    var dateFilterSplit = datefilter.Split(new[] { '-' }, 2);
                    if (dateFilterSplit.Length == 2)
                    {
                        try
                        {
                            startDate = DateTime.ParseExact(dateFilterSplit[0].Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                            endDate = DateTime.ParseExact(dateFilterSplit[1].Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture).AddDays(1).AddMilliseconds(-1);
                        }
                        catch (Exception)
                        {
                            startDate = DateTime.Now.AddMonths(-3);
                            endDate = DateTime.Now;
                        }
                    }
                }

                PAYEReceiptSearchParams searchData = new PAYEReceiptSearchParams
                {
                    ReceiptNumber = receiptNumber,
                    TaxEntityId = user.Entity.Id,
                    Skip = 0,
                    Take = 10,
                    EndDate = endDate,
                    StartDate = startDate
                };

                var vm = _payeReceiptUtilizationHandler.GetPAYEReceipts(searchData);
                vm.HeaderObj.DisplayText = user.Name;
                return View(vm);
            }
            catch (Exception exception) { Logger.Error(exception, exception.Message); }
            TempData = null;
            TempData.Add("Error", ErrorLang.genericexception().ToString());
            return RedirectToRoute(Module.Web.RouteName.Collection.GenerateInvoice);
        }

        /// <summary>
        /// Route name: C.PAYE.Receipt.Utilizations.Report
        /// Path: c/paye-receipt-utilizations/{receiptNumber}
        /// Get receipt utilizations list for a specified receipt number
        /// </summary>
        /// <param name="receiptNumber"></param>
        [BrowserHeaderFilter]
        public ActionResult ReceiptUtilizations(string receiptNumber)
        {
            string errorMessage = null;
            try
            {
                if (string.IsNullOrEmpty(receiptNumber)) { throw new NoRecordFoundException(); }
                PAYEReceiptUtilizationReportObj vm = _payeReceiptUtilizationHandler.GetUtilizationsReport(receiptNumber);
                if (vm != null)
                {
                    return View(vm);
                }
                else { errorMessage = $"Receipt Number {receiptNumber} was not found."; }
            }
            catch (NoRecordFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                errorMessage = ErrorLang.norecord404().ToString();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                errorMessage = ErrorLang.genericexception().ToString();
            }
            if (errorMessage != null) { TempData = null; TempData.Add("Error", errorMessage); }
            return RedirectToRoute(Module.Web.RouteName.Collection.GenerateInvoice);
        }

    }
}