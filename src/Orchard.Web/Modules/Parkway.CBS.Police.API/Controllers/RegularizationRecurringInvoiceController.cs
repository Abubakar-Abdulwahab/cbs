using System;
using System.IO;
using System.Web;
using System.Net;
using System.Text;
using System.Web.Http;
using Orchard.Logging;
using Parkway.CBS.Core.Lang;
using System.Collections.Generic;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.API.Controllers.Handlers.Contracts;

namespace Parkway.CBS.Police.API.Controllers
{
    [RoutePrefix("api/v1/pss/egs-regularization-recurring-invoice-payment-confirmation")]
    public class RegularizationRecurringInvoiceController : ApiController
    {
        private readonly IRegularizationRecurringInvoiceHandler _handler;
        public ILogger Logger { get; set; }

        public RegularizationRecurringInvoiceController(IRegularizationRecurringInvoiceHandler handler)
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
            { Logger.Error(exception.Message, "Exception Logging regularization recurring invoice request"); }
        }


        /// <summary>
        /// Do work for application fee has been confirmed
        /// </summary>
        [HttpPost]
        public IHttpActionResult ProcessingFeeConfirmation(string requestToken, string invoiceNumber)
        {
            try
            {
                LogRequest("RegularizationRecurringInvoice : ProcessingFeeConfirmation");
                IEnumerable<PSServiceRequestInvoiceValidationDTO> invoiceNumberGrp = _handler.ConfirmRequestInvoiceFee(requestToken, invoiceNumber);
                //we have confirmed the invoice fee to be fully paid for
                _handler.DoProcessing(invoiceNumberGrp);
                return Content(HttpStatusCode.OK, "Success");
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("RegularizationRecurringInvoiceFeeProcessing exception {0}", exception.Message));
                return Content(HttpStatusCode.OK, new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception() });
            }
        }


    }
}