using Orchard;
using Orchard.Logging;
using Orchard.UI.Admin;
using Orchard.UI.Notify;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Module.Controllers.Handlers.Contracts;
using System;
using System.Web.Mvc;

namespace Parkway.CBS.Module.Controllers
{
    [Admin]
    public class PaymentProviderValidationConstraintAJAXController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly INotifier _notifier;
        public ILogger Logger { get; set; }
        private readonly IPaymentProviderValidationConstraintHandler _handler;
        

        public PaymentProviderValidationConstraintAJAXController(IOrchardServices orchardServices, IPaymentProviderValidationConstraintHandler handler)
        {
            _orchardServices = orchardServices;
            _notifier = orchardServices.Notifier;
            Logger = NullLogger.Instance;
            _handler = handler;
        }


        public JsonResult UpdateStagingData(string additions, string removals, string providerId)
        {
            try
            {
                if (string.IsNullOrEmpty(additions) && string.IsNullOrEmpty(removals))
                {
                    return Json(new APIResponse { ResponseObject = "No update necessary." });
                }
                else
                {
                    return Json(_handler.UpdateStagingData(additions, removals, providerId));
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
            }
            return Json(new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception() });
        }


        public JsonResult GetRevenueHeadsPerMda(string mdaIds)
        {
            try
            {
                return Json(new APIResponse { Error = false, ResponseObject = _handler.GetRevenueHeadsPerMda(mdaIds) });
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
            }

            return Json(new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().ToString() });
        }

    }
}