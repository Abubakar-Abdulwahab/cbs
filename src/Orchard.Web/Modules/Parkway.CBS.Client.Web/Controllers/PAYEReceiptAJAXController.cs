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
    public class PAYEReceiptAJAXController : Controller
    {
        public ILogger Logger { get; set; }
        private readonly ICommonHandler _commonHandler;
        private readonly IPAYEReceiptUtilizationHandler _payeReceiptUtilizationHandler;

        public PAYEReceiptAJAXController(ICommonHandler commonHandler, IPAYEReceiptUtilizationHandler payeReceiptUtilizationHandler)
        {
            _commonHandler = commonHandler;
            _payeReceiptUtilizationHandler = payeReceiptUtilizationHandler;
            Logger = NullLogger.Instance;
        }

        public virtual JsonResult ReceiptsMoveRight(string token, int page)
        {
            Logger.Information(string.Format("getting page data for batch token - {0} page - {1}", "", page.ToString()));
            return Json(_payeReceiptUtilizationHandler.GetPagedPAYEReceiptData(token, page), JsonRequestBehavior.AllowGet);
        }

    }
}