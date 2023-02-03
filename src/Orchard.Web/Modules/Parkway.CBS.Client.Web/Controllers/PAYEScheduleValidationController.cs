using Orchard.Logging;
using Orchard.Themes;
using Parkway.CBS.Client.Web.Controllers.Handlers.Contracts;
using Parkway.CBS.Core.Lang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Parkway.CBS.Client.Web.Controllers
{
    [Themed]
    public class PAYEScheduleValidationController : Controller
    {
        public ILogger Logger { get; set; }
        private readonly ICommonHandler _commonHandler;

        public PAYEScheduleValidationController(ICommonHandler commonHandler)
        {
            _commonHandler = commonHandler;
            Logger = NullLogger.Instance;
        }

        [HttpGet]
        public ActionResult ValidatePAYESchedule()
        {
            try
            {
                return View(_commonHandler.GetHeaderObj());
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
            }
            TempData = null;
            TempData.Add("Error", ErrorLang.genericexception().ToString());
            return RedirectToRoute(Module.Web.RouteName.Collection.GenerateInvoice);
        }

        [HttpPost]
        public ActionResult ValidatePAYESchedule(string batchRef)
        {
            try
            {
                return RedirectToRoute(RouteName.TaxReceiptUtilization.ReceiptUtilization, new { scheduleBatchRef = batchRef });
            }catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
            }
            TempData = null;
            TempData.Add("Error", ErrorLang.genericexception().ToString());
            return RedirectToRoute(Module.Web.RouteName.Collection.GenerateInvoice);
        }
    }
}