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
    [RoutePrefix("api/v1/pss/deployment-allowance-settlement-engine-payment-callback")]
    public class RegularizationUnknownOfficersDeploymentAllowancePaymentSettlementEngineController : ApiController
    {
        private readonly IRegularizationUnknownOfficersDeploymentAllowancePaymentSettlementEngineHandler _regularizationUnknownOfficersDeploymentAllowancePaymentSettlementEngineHandler;
        public ILogger Logger { get; set; }

        public RegularizationUnknownOfficersDeploymentAllowancePaymentSettlementEngineController(IRegularizationUnknownOfficersDeploymentAllowancePaymentSettlementEngineHandler regularizationUnknownOfficersDeploymentAllowancePaymentSettlementEngineHandler)
        {
            Logger = NullLogger.Instance;
            _regularizationUnknownOfficersDeploymentAllowancePaymentSettlementEngineHandler = regularizationUnknownOfficersDeploymentAllowancePaymentSettlementEngineHandler;
        }

        /// <summary>
        /// Call back URL for the regularization unknown officers deployment allowance payment settlement engine notification
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult PaymentRequestStatusCallBack(SettlementEnginePaymentStatusVM model)
        {
            try
            {
                LogRequest("RegularizationDeploymentAllowancePaymentSettlementEngine : RegularizationDeploymentAllowancePaymentRequestStatusCallBack");

                if (model != null)
                {
                    APIResponse response = _regularizationUnknownOfficersDeploymentAllowancePaymentSettlementEngineHandler.ProcessPaymentRequestCallBack(model);

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

        /// <summary>
        /// Logs the request details
        /// </summary>
        /// <param name="endPointDescription"></param>
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