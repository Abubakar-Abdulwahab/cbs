using Orchard.Logging;
using Orchard.Themes;
using Parkway.CBS.Client.Web.Controllers.Handlers.Contracts;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Module.Web.Middleware.Filters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Parkway.CBS.Client.Web.Controllers
{
    [Themed]
    [CBSCollectionAuthorized]
    public class PAYEScheduleController : Controller
    {
        public ILogger Logger { get; set; }
        private readonly ICommonHandler _commonHandler;
        private readonly IPAYEScheduleHandler _payeScheduleHandler;

        public PAYEScheduleController(ICommonHandler commonHandler, IPAYEScheduleHandler payeScheduleHandler)
        {
            _commonHandler = commonHandler;
            _payeScheduleHandler = payeScheduleHandler;
            Logger = NullLogger.Instance;
        }


        public ActionResult PAYESchedules(string datefilter, string batchRef)
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

                PAYEBatchRecordSearchParams searchData = new PAYEBatchRecordSearchParams
                {
                    BatchRef = batchRef,
                    TaxEntityId = user.Entity.Id,
                    Skip = 0,
                    Take = 10,
                    EndDate = endDate,
                    StartDate = startDate
                };

                var vm = _payeScheduleHandler.GetPAYEBatchRecords(searchData);
                vm.HeaderObj.DisplayText = user.Name;
                return View(vm);
            }
            catch (Exception exception) { Logger.Error(exception, exception.Message); }
            TempData = null;
            TempData.Add("Error", ErrorLang.genericexception().ToString());
            return RedirectToRoute(Module.Web.RouteName.Collection.GenerateInvoice);
        }


        public ActionResult PAYEScheduleUtilizedReceipts(string batchRef)
        {
            string errorMessage = null;
            try
            {
                if (!string.IsNullOrEmpty(batchRef))
                {
                    return View(_payeScheduleHandler.GetUtilizedReceipts(batchRef.Trim()));
                }
                else { errorMessage = ErrorLang.norecord404().ToString(); }
            }catch(Exception exception) { Logger.Error(exception, exception.Message); }
            if (string.IsNullOrEmpty(errorMessage)) { errorMessage = ErrorLang.utilizedreceiptsnotfound(batchRef).ToString(); }
            TempData = null;
            TempData.Add("Error", errorMessage);
            return RedirectToRoute(Module.Web.RouteName.Collection.GenerateInvoice);
        }
    }
}