using System;
using System.IO;
using System.Web;
using System.Net;
using System.Text;
using System.Linq;
using System.Web.Http;
using Orchard.Logging;
using Parkway.CBS.Core.Lang;
using System.Collections.Generic;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.API.Controllers.Handlers.Contracts;

namespace Parkway.CBS.Police.API.Controllers
{
    [RoutePrefix("api/v1/pss/request")]
    public class RequestController : ApiController
    {
        private readonly IAPIRequestHandler _handler;
        public ILogger Logger { get; set; }

        public RequestController(IAPIRequestHandler handler)
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
            catch (Exception exception)
            { Logger.Error(exception.Message, "Exception ing Log Req"); }
        }


        /// <summary>
        /// Do work for application fee has been confirmed
        /// </summary>
        /// <returns><see cref="APIResponse"/></returns>
        [HttpPost]
        [Route("fee-confirmation")]
        public IHttpActionResult ProcessingFeeConfirmation(string requestToken, PaymentNotification notif)
        {
            try
            {
                LogRequest("ProcessingFeeConfirmation : ProcessingFeeConfirmation");
                IEnumerable<PSServiceRequestInvoiceValidationDTO> result = _handler.ConfirmRequestInvoiceFee(requestToken, notif.InvoiceNumber);
                //we have confirmed the invoice fee to be fully paid for
                IEnumerable<PSServiceRequestInvoiceValidationDTO> invoiceNumberGrp = result.Where(g => g.InvoiceNumber == notif.InvoiceNumber);
                //check if request can move to next level
                APIResponse response = _handler.DoProcessing(invoiceNumberGrp, _handler.SkipRequestMoveToNextStage(result, invoiceNumberGrp));
                return Content(HttpStatusCode.OK, response);                
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("CreateInvoice exception {0}", exception.Message));
                return Content(HttpStatusCode.OK, new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception() });
            }
        }


    }
}