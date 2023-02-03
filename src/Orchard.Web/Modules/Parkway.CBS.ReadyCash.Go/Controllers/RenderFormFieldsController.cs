using Orchard.Logging;
using Parkway.CBS.ReadyCash.Go.Controllers.Handlers.Contracts;
using System;
using System.Web.Mvc;
using Parkway.CBS.Core.StateConfig;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.ReadyCash.Go.Middleware.Filters;

namespace Parkway.CBS.ReadyCash.Go.Controllers
{
    [CORSHeaderFilter]
    public class RenderFormFieldsController : Controller
    {
        private readonly IRenderFormFieldsHandler _handler;
        public ILogger Logger { get; set; }

        public RenderFormFieldsController(IRenderFormFieldsHandler handler)
        {
            _handler = handler;
            Logger = NullLogger.Instance;
        }


        public ActionResult RenderFormField(string revenueHead, string payerId)
        {
            int revenueHeadId = 0;
            string errorMessage = string.Empty;
            try
            {
                var billerCode = HttpContext.Request.Headers.Get("BILLERCODE");
                StateConfig siteConfig = _handler.GetTenantConfig(billerCode);
                if (siteConfig == null)
                {
                    errorMessage = $"Biller code {billerCode} not found"; throw new Exception();
                }

                if (string.IsNullOrEmpty(revenueHead) || string.IsNullOrEmpty(payerId)) { errorMessage = "Specified revenue head or payer id is empty."; throw new Exception(); }
                if (!int.TryParse(revenueHead, out revenueHeadId)) { errorMessage = "Unable to parse RevenueHeadId."; throw new Exception(); }

                return View("RenderFormField", _handler.GetFormControlsForRevenueHead(revenueHeadId, payerId, siteConfig.FileSiteName));
            }
            catch (NoRecordFoundException exception)
            {
                Logger.Error(exception, $"{errorMessage}. Exception Message ---- {exception.Message}");
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.NoContent);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, $"{errorMessage}. Exception Message ---- {exception.Message}");
            }
            return new HttpNotFoundResult();
        }
    }
}