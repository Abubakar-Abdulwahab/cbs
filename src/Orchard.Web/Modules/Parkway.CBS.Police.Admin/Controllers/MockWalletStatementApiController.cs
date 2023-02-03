using Orchard.Logging;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Parkway.CBS.Police.Admin.Controllers
{
    public class MockWalletStatementApiController : Controller
    {
        private readonly IMockWalletStatementApiHandler _handler;
        ILogger Logger { get; set; }
        public MockWalletStatementApiController(IMockWalletStatementApiHandler handler)
        {
            _handler = handler;
            Logger = NullLogger.Instance;
        }


        [HttpPost]
        public JsonResult GetWalletStatements(int Skip, string StartDate, string EndDate)
        {
            try
            {
                return Json(new { Error = false, ErrorMessage = "", items = _handler.GetStatements(Skip) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                return Json(new { Error = true, ErrorMessage = "something went wrong" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}