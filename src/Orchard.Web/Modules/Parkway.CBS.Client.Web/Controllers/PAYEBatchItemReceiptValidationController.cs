using Newtonsoft.Json;
using Orchard;
using Orchard.Logging;
using Orchard.Security;
using Orchard.Themes;
using Orchard.Users.Events;
using Orchard.Users.Services;
using Parkway.CBS.Client.Web.Controllers.Handlers.Contracts;
using Parkway.CBS.Client.Web.RouteName;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Module.Web.Controllers;
using Parkway.CBS.Module.Web.Controllers.Handlers.Contracts;
using Parkway.CBS.Module.Web.Middleware.Filters;
using Parkway.CBS.Module.Web.ViewModels;
using System;
using System.Web.Mvc;

namespace Parkway.CBS.Client.Web.Controllers
{
    [Themed]
    public class PAYEBatchItemReceiptValidationController : Controller
    {
        public ILogger Logger { get; set; }
        private readonly IPAYEBatchItemReceiptValidationHandler _receiptValidationHandler;
        private readonly ICommonHandler _commonHandler;
        private readonly IOrchardServices _orchardServices;

        public PAYEBatchItemReceiptValidationController(IPAYEBatchItemReceiptValidationHandler receiptValidationHandler, ICommonHandler commonHandler, IOrchardServices orchardServices)
        {
            Logger = NullLogger.Instance;
            _receiptValidationHandler = receiptValidationHandler;
            _commonHandler = commonHandler;
            _orchardServices = orchardServices;
        }


        /// <summary>
        /// Route name: C.PAYE.BatchItem.Receipt.Validate
        /// Path: c/paye-tax-receipt-validate
        /// </summary>
        /// <returns></returns>
        [BrowserHeaderFilter]
        public ActionResult ValidateReceipt()
        {
            string message = string.Empty;
            bool hasError = false;
            string receiptNumber = null;

            try
            {
                if (TempData.ContainsKey("NoPAYEReceipt"))
                {
                    receiptNumber = TempData["NoPAYEReceipt"].ToString();
                    message = ErrorLang.receipt404(receiptNumber).ToString();
                    hasError = true;
                    TempData.Remove("NoPAYEReceipt");
                }
            }
            catch (Exception exception)
            { Logger.Error(exception, exception.Message); }

            TempData = null;
            PAYESearchByReceiptNumberVM obj = new PAYESearchByReceiptNumberVM { HeaderObj = _commonHandler.GetHeaderObj(), ErrorMessage = message, HasErrors = hasError, ReceiptNumber = receiptNumber };
            return View(obj);
        }

        /// <summary>
        /// Route name: C.PAYE.BatchItem.Receipt.Validate
        /// Path: c/paye-tax-receipt-validate
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, ActionName("ValidateReceipt")]
        public virtual ActionResult ValidateReceipt(PAYESearchByReceiptNumberVM model)
        {
            try
            {
                if (!string.IsNullOrEmpty(model.ReceiptNumber))
                {
                    return RedirectToRoute(PAYEBatchItemReceiptValidation.PAYEBatchItemReceiptDetails, new { receiptNumber = model.ReceiptNumber.Trim() });
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Error while retriving receipt {0}", model.ReceiptNumber));
            }

            Logger.Error("Receipt not found " + model.ReceiptNumber);
            TempData = null;
            TempData.Add("NoPAYEReceipt", model.ReceiptNumber);
            return RedirectToRoute(PAYEBatchItemReceiptValidation.ValidatePAYEBatchItemReceipt);
        }

        /// <summary>
        /// Route name: C.PAYE.BatchItem.Receipt.Details
        /// Path: c/paye-tax-receipt-details/{receiptNumber}
        /// </summary>
        /// <param name="receiptNumber"></param>
        [BrowserHeaderFilter]
        public ActionResult ReceiptDetails(string receiptNumber)
        {
            try
            {
                PAYEBatchItemReceiptViewModel receiptDetails = _receiptValidationHandler.GetPAYEBatchItemReceiptVM(receiptNumber);
                return View(new PAYESearchByReceiptNumberVM { HeaderObj = _commonHandler.GetHeaderObj(), ReceiptViewModel = receiptDetails, ReceiptNumber = receiptNumber });
            }
            catch (NoRecordFoundException ex)
            {
                Logger.Error(ex, string.Format("No record found for PAYE receipt number {0} {1}", receiptNumber, ex.Message));
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Error searching for PAYE receipt number {0} {1}", receiptNumber, exception.Message));
            }

            Logger.Error("Receipt not found " + receiptNumber);
            TempData = null;
            TempData.Add("NoPAYEReceipt", receiptNumber);
            return RedirectToRoute(PAYEBatchItemReceiptValidation.ValidatePAYEBatchItemReceipt);
        }

        /// <summary>
        /// Generate PAYE Receipt PDF
        /// </summary>
        /// <param name="receiptNumber"></param>
        /// <returns></returns>
        public ActionResult GetReceipt(string receiptNumber)
        {
            try
            {
                CreateReceiptDocumentVM result = _receiptValidationHandler.CreateReceiptFile(receiptNumber);
                System.Web.HttpContext.Current.Response.ClearHeaders();
                System.Web.HttpContext.Current.Response.ContentType = "application/pdf";
                System.Web.HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + result.FileName);
                System.Web.HttpContext.Current.Response.TransmitFile(result.SavedPath);
                System.Web.HttpContext.Current.Response.End();
                return RedirectToRoute(PAYEBatchItemReceiptValidation.PAYEBatchItemReceiptDetails, new { receiptNumber = receiptNumber.Trim() });
            }
            catch (UserNotAuthorizedForThisActionException exception)
            {
                var message = String.Format("\nUser ID {0} tried to View PAYE Receipt without permission", _orchardServices.WorkContext.CurrentUser.UserName);
                Logger.Error(exception, message + exception.Message);
            }
            catch (NoRecordFoundException exception)
            {
                Logger.Error(exception, exception.Message);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ex.Message);
            }

            Logger.Error("Receipt not found " + receiptNumber);
            TempData = null;
            TempData.Add("NoPAYEReceipt", receiptNumber);
            return RedirectToRoute(RouteName.PAYEBatchItemReceiptValidation.ValidatePAYEBatchItemReceipt);
        }
    }
}