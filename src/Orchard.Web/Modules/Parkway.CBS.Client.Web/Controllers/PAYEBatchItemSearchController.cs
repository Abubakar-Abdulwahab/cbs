using Orchard;
using Orchard.Logging;
using Orchard.Themes;
using Parkway.CBS.Client.Web.Controllers.Handlers.Contracts;
using Parkway.CBS.Client.Web.RouteName;
using Parkway.CBS.Core.HelperModels;
using System;
using System.Web.Mvc;

namespace Parkway.CBS.Client.Web.Controllers
{
    [Themed]
    public class PAYEBatchItemSearchController : Controller
    {
        private readonly ICommonHandler _commonHandler;
        private readonly IOrchardServices _orchardServices;
        private readonly IPAYEBatchItemSearchHandler _handler;
        public ILogger Logger { get; set; }

        public PAYEBatchItemSearchController(ICommonHandler commonHandler, IOrchardServices orchardServices, IPAYEBatchItemSearchHandler handler)
        {
            Logger = NullLogger.Instance;
            _commonHandler = commonHandler;
            _orchardServices = orchardServices;
            _handler = handler;
        }

        [HttpGet]
        public ActionResult PAYEBatchValidation()
        {
            string message = string.Empty;
            bool hasError = false;

            try
            {
                if (TempData.ContainsKey("NoPAYEBatch"))
                {
                    message = TempData["NoPAYEBatch"].ToString();
                    hasError = true;
                    TempData.Remove("NoPAYEBatch");
                }
            }
            catch (Exception exception)
            { Logger.Error(exception, exception.Message); }

            TempData = null;
            return View(new PAYESearchByBatchRefVM { HeaderObj = _commonHandler.GetHeaderObj(), ErrorMessage = message, HasErrors = hasError });
        }


        [HttpPost]
        public ActionResult PAYEBatchValidation(PAYESearchByBatchRefVM model)
        {
            string errorMessage = null;
            try
            {
                if (!string.IsNullOrEmpty(model.BatchRef))
                {
                    return RedirectToRoute(PAYEBatchItemSearch.PAYEBatchItems, new { batchRef = model.BatchRef.Trim() });
                }
            }
            catch (Exception exception)
            {
                errorMessage = string.Format("Error while retriving PAYE Batch Items for Batch with ref {0}", model.BatchRef);
                Logger.Error(exception, exception.Message);
            }

            errorMessage = (string.IsNullOrEmpty(errorMessage)) ? "Batch ref not specified" : errorMessage;
            Logger.Error(errorMessage);
            TempData = null;
            TempData.Add("NoPAYEBatch", errorMessage);

            return RedirectToRoute(PAYEBatchItemSearch.PAYEBatchValidation);
        }


        [HttpGet]
        public ActionResult PAYEBatchItems(string batchRef)
        {
            string errorMessage = null;
            try
            {
                if (!string.IsNullOrEmpty(batchRef))
                {
                    var batchItems = _handler.GetPAYEBatchItemsListVM(batchRef.Trim(), 1);
                    batchItems.HeaderObj = _commonHandler.GetHeaderObj();
                    return View(batchItems);
                }
            }
            catch(Exception exception)
            {
                errorMessage = string.Format("Error while retriving PAYE Batch Items for Batch with ref {0}", batchRef);
                Logger.Error(exception, exception.Message);
            }

            errorMessage = (string.IsNullOrEmpty(errorMessage)) ? "Batch ref not specified" : errorMessage;
            Logger.Error(errorMessage);
            TempData = null;
            TempData.Add("NoPAYEBatch", errorMessage);

            return RedirectToRoute(PAYEBatchItemSearch.PAYEBatchValidation);
        }
    }
}