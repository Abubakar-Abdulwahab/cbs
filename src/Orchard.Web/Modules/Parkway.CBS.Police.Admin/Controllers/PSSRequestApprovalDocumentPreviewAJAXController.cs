using Orchard;
using Orchard.Logging;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Parkway.CBS.Police.Admin.Controllers
{
    public class PSSRequestApprovalDocumentPreviewAJAXController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IPSSRequestApprovalDocumentPreviewHandler _handler;
        ILogger Logger { get; set; }
        public PSSRequestApprovalDocumentPreviewAJAXController(IPSSRequestApprovalDocumentPreviewHandler handler, IOrchardServices orchardServices)
        {
            _orchardServices = orchardServices;
            _handler = handler;
            Logger = NullLogger.Instance;
        }


        public JsonResult ConfirmAdminHasViewedDraftDocument(string fileRefNumber)
        {
            try
            {
                return Json(_handler.ConfirmAdminHasViewedDraftDocument(fileRefNumber, _orchardServices.WorkContext.CurrentUser.Id), JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                return Json(new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().Text });
            }
        }
    }
}