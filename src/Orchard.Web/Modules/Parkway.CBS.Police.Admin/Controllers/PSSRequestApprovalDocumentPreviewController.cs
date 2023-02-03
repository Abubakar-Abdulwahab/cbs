using Orchard;
using Orchard.Logging;
using Orchard.UI.Admin;
using Orchard.UI.Notify;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Parkway.CBS.Police.Admin.Controllers
{
    public class PSSRequestApprovalDocumentPreviewController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly INotifier _notifier;
        private readonly IPSSRequestApprovalDocumentPreviewHandler _handler;
        ILogger Logger { get; set; }

        public PSSRequestApprovalDocumentPreviewController(IOrchardServices orchardServices, IPSSRequestApprovalDocumentPreviewHandler handler)
        {
            _orchardServices = orchardServices;
            _notifier = orchardServices.Notifier;
            _handler = handler;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// View draft service document
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <returns></returns>
        public ActionResult ViewDraftServiceDocument(string fileRefNumber)
        {
            try
            {
                if (string.IsNullOrEmpty(fileRefNumber) || fileRefNumber.Trim().Length == 0) { throw new Exception("File ref number not specified"); }
                CreateCertificateDocumentVM result = _handler.CreateDraftServiceDocument(fileRefNumber);
                return File(result.DocByte, "application/pdf");
            }
            catch (NoBillingTypeSpecifiedException exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.norecord404());
            }
            catch (Exception ex)
            {
                _notifier.Error(ErrorLang.genericexception());
                Logger.Error(ex, ex.Message);
            }
            return Redirect("~/Admin");
        }
    }
}