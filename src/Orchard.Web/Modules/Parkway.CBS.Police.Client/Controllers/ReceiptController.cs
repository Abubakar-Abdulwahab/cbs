using System;
using System.Web.Mvc;
using Orchard.Security;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;
using Parkway.CBS.Core.Models.Enums;
using Orchard.Logging;
using Parkway.CBS.Core.Utilities;
using System.Linq;
using Orchard;
using Parkway.CBS.Core.Exceptions;
using System.Threading.Tasks;
using Parkway.CBS.Core.Lang;
using System.Net.Http;
using Newtonsoft.Json;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Client.Controllers
{
    public class ReceiptController : BaseController
    {
        private readonly IHandler _compHandler;
        private readonly IAuthenticationService _authenticationService;
        private readonly IPSSReceiptHandler _handler;
        private readonly IOrchardServices _orchardServices;


        public ReceiptController(IHandler compHandler,IOrchardServices orchardServices, IAuthenticationService authenticationService, IPSSReceiptHandler handler)
            : base(authenticationService, compHandler)
        {
            _authenticationService = authenticationService;
            _handler = handler;
            Logger = NullLogger.Instance;
            _orchardServices = orchardServices;
            _compHandler = compHandler;
        }


        /// <summary>
        /// Route name: C.Invoice.ReceiptDetails
        /// URL: p/invoice/receipts/{invoiceNumber}
        /// </summary>
        /// <param name="invoiceNumber"></param>
        public virtual ActionResult Receipts(string invoiceNumber)
        {
            try
            {
                ReceiptDisplayVM invoiceDetails = _handler.SearchForInvoiceForPaymentView(invoiceNumber);
                if (invoiceDetails == null)
                {
                    Logger.Error("Invoice not found " + invoiceNumber);
                    TempData = null;
                    TempData.Add("NoInvoice", invoiceNumber);
                    return RedirectToRoute("P.BIN.Search");
                }

                UserDetailsModel user = GetLoggedInUserDetails();
                HeaderObj headerObj = new HeaderObj { ShowSignin = true };
                if (user != null && user.Entity != null) { headerObj.IsLoggedIn = true; headerObj.DisplayText = user.Name; }
                invoiceDetails.HeaderObj = headerObj;

                return View(invoiceDetails);
            }
            catch (NoRecordFoundException ex)
            {
                Logger.Error(ex, string.Format("No record found for invoice number {0} {1}", invoiceNumber, ex.Message));
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Error searching for receipt with invoice number {0} {1}", invoiceNumber, exception.Message));
            }

            Logger.Error("Invoice receipt not found " + invoiceNumber);
            TempData = null;
            TempData.Add("NoInvoice", invoiceNumber);
            return RedirectToRoute("P.BIN.Search");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="receiptNumber"></param>
        /// <returns></returns>
        public virtual ActionResult ReceiptDetails(string invoiceNumber, string receiptNumber)
        {
            try
            {
                ReceiptDetailsVM receiptDetails = null;

                if (TempData.ContainsKey("ReceiptDetails"))
                {
                    receiptDetails = JsonConvert.DeserializeObject<ReceiptDetailsVM>(TempData["ReceiptDetails"].ToString());
                    TempData.Remove("ReceiptDetails");
                }
                else
                {
                    receiptDetails = _handler.GetReceiptVM(invoiceNumber, receiptNumber);
                    if(receiptDetails.Transactions == null || receiptDetails.Transactions.Count == 0)
                    {
                        Logger.Error("Invoice not found " + invoiceNumber);
                        TempData = null;
                        TempData.Add("NoInvoice", invoiceNumber);
                        return RedirectToRoute("P.BIN.Search");
                    }
                }

                TempData = null;

                UserDetailsModel user = GetLoggedInUserDetails();
                HeaderObj headerObj = new HeaderObj { ShowSignin = true };
                if (user != null && user.Entity != null) { headerObj.IsLoggedIn = true; headerObj.DisplayText = user.Name; }
                receiptDetails.HeaderObj = headerObj;
                return View(receiptDetails);
            }
            catch (NoRecordFoundException ex)
            {
                Logger.Error(ex, string.Format("No record found for receipt number {0} {1}", receiptNumber, ex.Message));
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Error searching for receipt number {0} {1}", receiptNumber, exception.Message));
            }

            Logger.Error("Invoice not found " + invoiceNumber);
            TempData = null;
            TempData.Add("NoInvoice", invoiceNumber);
            return RedirectToRoute("P.BIN.Search");
        }


        /// <summary>
        /// Generate pdf receipt
        /// </summary>
        /// <param name="ReceiptNumber"></param>
        public ActionResult GetReceipt(string invoiceNumber, string ReceiptNumber)
        {
            try
            {
                CreateReceiptDocumentVM result = _handler.CreateReceiptFile(invoiceNumber, ReceiptNumber);
                System.Web.HttpContext.Current.Response.ClearHeaders();
                System.Web.HttpContext.Current.Response.ContentType = "application/pdf";
                System.Web.HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + result.FileName);
                System.Web.HttpContext.Current.Response.TransmitFile(result.SavedPath);
                System.Web.HttpContext.Current.Response.End();
                return RedirectToRoute("P.ReceiptDetails", new { invoiceNumber = invoiceNumber, receiptNumber = ReceiptNumber });
            }
            catch (UserNotAuthorizedForThisActionException exception)
            {
                var message = String.Format("\nUser ID {0} tried to ViewReceipt without permission", _orchardServices.WorkContext.CurrentUser.UserName);
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

            Logger.Error("Invoice not found " + invoiceNumber);
            TempData = null;
            TempData.Add("NoInvoice", invoiceNumber);
            return RedirectToRoute("P.BIN.Search");
        }

    }
}