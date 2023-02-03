using Orchard.Logging;
using Orchard.Themes;
using Parkway.CBS.Client.Web.Controllers.Handlers.Contracts;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Module.Web.Middleware.Filters;
using Parkway.CBS.Module.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Parkway.CBS.Client.Web.Controllers
{
    [CBSCollectionAJAXAuthorized]
    public class PAYEBatchItemReceiptAJAXController : Controller
    {
        public ILogger Logger { get; set; }
        private readonly ICommonHandler _commonHandler;
        private readonly IPAYEBatchItemReceiptHandler _payeReceiptHandler;

        public PAYEBatchItemReceiptAJAXController(ICommonHandler commonHandler, IPAYEBatchItemReceiptHandler payeReceiptHandler)
        {
            _commonHandler = commonHandler;
            _payeReceiptHandler = payeReceiptHandler;
            Logger = NullLogger.Instance;
        }

        public virtual JsonResult ReceiptsMoveRight(string token, int page)
        {
            Logger.Information(string.Format("getting page data for batch token - {0} page - {1}", "", page.ToString()));
            return Json(_payeReceiptHandler.GetPagedBatchItemReceiptData(token, page), JsonRequestBehavior.AllowGet);
        }

    }
}