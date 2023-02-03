using Orchard.Logging;
using Orchard.Themes;
using Parkway.CBS.Client.Web.Controllers.Handlers.Contracts;
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
    public class PAYEBatchItemReceiptController : Controller
    {
        public ILogger Logger { get; set; }
        private readonly ICommonHandler _commonHandler;
        private readonly IPAYEBatchItemReceiptHandler _payeReceiptHandler;

        public PAYEBatchItemReceiptController(ICommonHandler commonHandler, IPAYEBatchItemReceiptHandler payeReceiptHandler)
        {
            _commonHandler = commonHandler;
            _payeReceiptHandler = payeReceiptHandler;
            Logger = NullLogger.Instance;
        }

        [CBSCollectionAuthorized]
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
                    IsEmployer = user.Entity.TaxEntityCategory.GetSettings().IsEmployer,
                    EndDate = endDate,
                    StartDate = startDate
                };

                var vm = _payeReceiptHandler.GetPAYEBatchItemReceipts(searchData);
                vm.HeaderObj.DisplayText = user.Name;
                return View(vm);
            }
            catch (Exception exception) { Logger.Error(exception, exception.Message); }
            TempData = null;
            TempData.Add("Error", ErrorLang.genericexception().ToString());
            return RedirectToRoute(Module.Web.RouteName.Collection.GenerateInvoice);
        }


        public ActionResult Receipt()
        {
            try
            {
                return View();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
            }
            return RedirectToRoute(Module.Web.RouteName.Collection.GenerateInvoice);
        }
    }
}