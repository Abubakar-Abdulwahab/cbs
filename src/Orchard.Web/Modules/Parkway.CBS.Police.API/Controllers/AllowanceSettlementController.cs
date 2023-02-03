using System;
using System.IO;
using System.Web;
using System.Net;
using System.Text;
using System.Web.Http;
using Orchard.Logging;
using Parkway.CBS.Police.API.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;

namespace Parkway.CBS.Police.API.Controllers
{
    [RoutePrefix("api/v1/pss/notification")]
    public class AllowanceSettlementController : ApiController
    {
        private readonly IAllowanceSettlementHandler _handler;
        public ILogger Logger { get; set; }
        private IOrchardServices _orchardServices;

        public AllowanceSettlementController(IOrchardServices orchardServices, IAllowanceSettlementHandler handler)
        {
            _orchardServices = orchardServices;
            _handler = handler;
            Logger = NullLogger.Instance;
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
            { Logger.Error(exception.Message, "Exception ing Log Req"); }
        }


        /// <summary>
        /// Update the deployment allowance payment status
        /// </summary>
        /// <returns><see cref="APIResponse"/></returns>
        [HttpPost]
        [Route("deployment-allowance-payment-status")]
        public IHttpActionResult UpdateStatus(DeploymentAllowancePaymentNotificationModel model)
        {
            try
            {
                LogRequest("DeploymentAllowancePaymentStatusNotification : DeploymentAllowancePaymentStatusNotification");
                if(model == null)
                {
                    return Content(HttpStatusCode.BadRequest, new APIResponse { Error = true, ResponseObject = "Model is empty" });
                }

                APIResponse response = _handler.UpdateDeploymentAllowanceStatus(model);
                return Content(response.StatusCode, response);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Deployment allowance settlement status Exception {0}", exception.Message));
                return Content(HttpStatusCode.BadRequest, new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().Text });
            }
        }

    }
}