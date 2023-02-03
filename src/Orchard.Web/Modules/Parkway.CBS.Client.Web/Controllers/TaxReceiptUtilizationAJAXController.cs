using Orchard.Logging;
using Parkway.CBS.Client.Web.Controllers.Handlers.Contracts;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Module.Web.Controllers;
using Parkway.CBS.Module.Web.Controllers.Handlers.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Parkway.CBS.Client.Web.Controllers
{
    public class TaxReceiptUtilizationAJAXController : BaseLiteController
    {
        private readonly ITaxReceiptUtilizationHandler _handler;
        public ILogger Logger { get; set; }

        public TaxReceiptUtilizationAJAXController(IHandlerComposition handlerComposition, ITaxReceiptUtilizationHandler handler) : base(handlerComposition)
        {
            _handler = handler;
            Logger = NullLogger.Instance;
        }

        /// <summary>
        /// Get PAYE Receipt
        /// </summary>
        /// <param name="receiptNumber"></param>
        /// <returns></returns>
        public JsonResult GetPAYEReceipt(string receiptNumber)
        {
            string errorMessage = null;
            try
            {
                if (!string.IsNullOrEmpty(receiptNumber))
                {
                    return Json(_handler.GetPAYEReceipt(receiptNumber.Trim(), GetLoggedInUserDetails().Entity.Id));
                }
                errorMessage = ErrorLang.norecord404("Receipt number not specified").Text;
            }
            catch (NoRecordFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                errorMessage = ErrorLang.norecord404(exception.Message).Text;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                errorMessage = ErrorLang.genericexception().Text;
            }
            return Json(new APIResponse { ResponseObject = errorMessage, Error = true });
        }

        /// <summary>
        /// Apply receipt to batch
        /// </summary>
        /// <param name="receiptNumber"></param>
        /// <param name="batchRef"></param>
        /// <returns></returns>
        public JsonResult ApplyReceiptToBatch(string receiptNumber, string batchRef)
        {
            string errorMessage = null;
            try
            {
                return Json(_handler.ApplyReceiptToBatch(receiptNumber.Trim(), batchRef.Trim(), GetLoggedInUserDetails().Entity.Id));
            }
            catch (NoRecordFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                errorMessage = ErrorLang.norecord404(exception.Message).Text;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                errorMessage = ErrorLang.genericexception().Text;
            }
            return Json(new APIResponse { ResponseObject = errorMessage, Error = true });
        }

        /// <summary>
        /// Get outstanding amount of schedule with specified batch ref
        /// </summary>
        /// <param name="batchRef"></param>
        /// <returns></returns>
        public JsonResult GetOutstandingAmount(string batchRef)
        {
            string errorMessage = null;
            try
            {
                if (!string.IsNullOrEmpty(batchRef))
                {
                    var vm = _handler.GetVM(batchRef);
                    return Json(new APIResponse { ResponseObject = new { Amount = vm.OutstandingAmount.ToString("N2"), Completed = vm.PaymentCompleted } });
                }
                else { throw new Exception("Schedule Batch ref not specified"); }
            }
            catch (NoRecordFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                errorMessage = ErrorLang.norecord404(exception.Message).Text;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                errorMessage = ErrorLang.genericexception().Text;
            }
            return Json(new APIResponse { ResponseObject = errorMessage, Error = true });
        }
    }
}