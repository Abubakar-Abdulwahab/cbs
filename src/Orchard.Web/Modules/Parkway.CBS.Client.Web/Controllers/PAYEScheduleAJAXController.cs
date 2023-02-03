using Orchard.Logging;
using Parkway.CBS.Client.Web.Controllers.Handlers.Contracts;
using Parkway.CBS.Module.Web.Middleware.Filters;
using System.Web.Mvc;

namespace Parkway.CBS.Client.Web.Controllers
{
    [CBSCollectionAJAXAuthorized]
    public class PAYEScheduleAJAXController : Controller
    {
        public ILogger Logger { get; set; }
        private readonly IPAYEScheduleHandler _payeScheduleHandler;

        public PAYEScheduleAJAXController(IPAYEScheduleHandler payeScheduleHandler)
        {
            _payeScheduleHandler = payeScheduleHandler;
            Logger = NullLogger.Instance;
        }

        public JsonResult BatchRecordsMoveRight(string token, int page)
        {
            Logger.Information(string.Format("getting page data for batch records token - {0} page - {1}", "", page.ToString()));
            return Json(_payeScheduleHandler.GetPagedBatchRecordsData(token, page), JsonRequestBehavior.AllowGet);
        }
    }
}