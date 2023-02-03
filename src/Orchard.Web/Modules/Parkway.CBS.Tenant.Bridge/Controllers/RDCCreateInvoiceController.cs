using System.IO;
using System.Web;
using System.Text;
using Orchard.Logging;
using System.Web.Http;
using Microsoft.Web.Http;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Tenant.Bridge.Controllers.Handlers.Contracts;


namespace Parkway.CBS.Tenant.Bridge.Controllers
{
    [ApiVersion("1.0")]
    [RoutePrefix("v1/bridge/agent/rdc/invoice")]
    public class RDCCreateInvoiceController : ApiController
    {
        private readonly IRDCCreateInvoiceHandler _handler;
        public ILogger Logger { get; set; }


        public RDCCreateInvoiceController(IRDCCreateInvoiceHandler handler)
        {
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
            catch (System.Exception exception)
            { Logger.Error(exception.Message, "Exception ing Log Req"); }
        }


        /// <summary>
        /// generate invoice for this agent
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("create")]
        public IHttpActionResult BillerCreateInvoice(RDCBillerCreateInvoiceModel model)
        {
            try
            {
                LogRequest("AgentCreateInvoiceController : AgentCreateInvoice");
                var signature = HttpContext.Current.Request.Headers.Get("SIGNATURE");
                var clientID = HttpContext.Current.Request.Headers.Get("CLIENTID");
                var billerCode = HttpContext.Current.Request.Headers.Get("BILLERCODE");

                APIResponse response = _handler.GenerateInvoice(model, new { SIGNATURE = signature, CLIENTID = clientID, BILLERCODE = billerCode });
                return Content(response.StatusCode, response);
            }
            catch (System.Exception exception)
            {
                Logger.Error(exception, string.Format("ValidateInvoiceNumber exception {0}", exception.Message));
                return Content(System.Net.HttpStatusCode.BadRequest, new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().ToString() });
            }
        }


    }
}
