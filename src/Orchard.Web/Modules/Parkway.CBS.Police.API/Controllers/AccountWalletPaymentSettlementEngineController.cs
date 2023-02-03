using Orchard.Logging;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Police.API.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Http;

namespace Parkway.CBS.Police.API.Controllers
{
    [RoutePrefix("api/v1/pss/account-wallet-payment-settlement-engine-call-back")]
    public class AccountWalletPaymentSettlementEngineController : ApiController
    {
        private readonly IAccountWalletPaymentSettlementEngineHandler _paymentSettlementEngineHandler;
        public ILogger Logger { get; set; }

        public AccountWalletPaymentSettlementEngineController(IAccountWalletPaymentSettlementEngineHandler paymentSettlementEngineHandler)
        {
            Logger = NullLogger.Instance;
            _paymentSettlementEngineHandler = paymentSettlementEngineHandler;
        }

        // GET: PaymentRequestStatusCallBack
        [HttpPost]
        [Route("callback")]
        public IHttpActionResult PaymentRequestStatusCallBack(SettlementEnginePaymentStatusVM model)
        {
            try
            {
                LogRequest("AccountWalletPaymentSettlementEngine : PaymentRequestStatusCallBack");

                if (model != null)
                {
                    APIResponse response = _paymentSettlementEngineHandler.ProcessPaymentRequestCallBack(model);

                    return Content(response.StatusCode, response);

                }

                return Content(HttpStatusCode.BadRequest, new APIResponse { Error = true, ResponseObject = ErrorLang.ToLocalizeString("Invalid request").Text });

            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
            }

            return Content(HttpStatusCode.InternalServerError, new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().Text });
        }

        private void LogRequest(string endPointDescription)
        {
            try
            {
                string requestBody = string.Empty;
                string IP = HttpContext.Current.Request.UserHostAddress;

                using (var stream = new MemoryStream())
                {
                    var context = (HttpContextBase)Request.Properties["MS_HttpContext"];
                    context.Request.InputStream.Seek(0, SeekOrigin.Begin);
                    context.Request.InputStream.CopyTo(stream);
                    requestBody = Encoding.UTF8.GetString(stream.ToArray());
                }
                Logger.Information(string.Format("{0} dump {1} IP: {2} ", endPointDescription, requestBody, IP));
            }
            catch (Exception exception)
            { Logger.Error(exception.Message, "Exception in Log Req"); }
        }

    }
}