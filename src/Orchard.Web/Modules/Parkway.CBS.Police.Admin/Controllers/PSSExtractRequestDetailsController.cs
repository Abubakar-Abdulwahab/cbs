using Orchard;
using Orchard.Logging;
using Orchard.UI.Notify;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using System;
using System.Web.Mvc;

namespace Parkway.CBS.Police.Admin.Controllers
{
    public class PSSExtractRequestDetailsController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IPSSExtractRequestDetailsHandler _handler;
        private readonly INotifier _notifier;
        ILogger Logger { get; set; }

        public PSSExtractRequestDetailsController(IOrchardServices orchardServices, IPSSExtractRequestDetailsHandler handler)
        {
            _orchardServices = orchardServices;
            _handler = handler;
            _notifier = orchardServices.Notifier;
            Logger = NullLogger.Instance;
        }

        
        public ActionResult ViewExtract(string fileRefNumber)
        {
            try
            {
                CreateCertificateDocumentVM result = _handler.CreateExtractDocumentByteFile(fileRefNumber);
                System.Web.HttpContext.Current.Response.ClearHeaders();
                System.Web.HttpContext.Current.Response.ContentType = "application/pdf";
                System.Web.HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + result.FileName);
                System.Web.HttpContext.Current.Response.TransmitFile(result.SavedPath);
                System.Web.HttpContext.Current.Response.End();
                return null;
            }
            catch (UserNotAuthorizedForThisActionException exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.usernotauthorized());
            }
            catch (NoRecordFoundException exception)
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