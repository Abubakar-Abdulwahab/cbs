using Orchard.Logging;
using Parkway.CBS.Client.Web.Controllers.Handlers.Contracts;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using System;
using System.Web.Mvc;

namespace Parkway.CBS.Client.Web.Controllers
{
    public class PAYEBatchItemSearchAJAXController : Controller
    {
        public ILogger Logger { get; set; }
        private readonly IPAYEBatchItemSearchHandler _handler;
        public PAYEBatchItemSearchAJAXController(IPAYEBatchItemSearchHandler handler)
        {
            Logger = NullLogger.Instance;
            _handler = handler;
        }


        public JsonResult BatchItemsMoveRight(string batchRef, int page)
        {
            try
            {
                Logger.Information(string.Format("getting paged batch items data for batch with ref - {0} page - {1}", batchRef, page.ToString()));
                var model = _handler.GetPAYEBatchItemsListVM(batchRef, page);
                return Json(new APIResponse { ResponseObject = new { DataSize = model.DataSize, BatchItems = model.BatchItems } }, JsonRequestBehavior.AllowGet);
            }catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
            }
            return Json(new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().ToString() });
            
        }
    }
}