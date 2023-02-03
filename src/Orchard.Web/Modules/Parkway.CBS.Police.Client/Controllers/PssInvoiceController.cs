using System;
using System.Web.Mvc;
using Orchard.Logging;
using Orchard.Security;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.VM;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.HelperModels;
using Newtonsoft.Json;
using Parkway.CBS.Core.Exceptions;

namespace Parkway.CBS.Police.Client.Controllers
{
    public class PssInvoiceController : BaseController
    {
        private readonly IHandler _compHandler;
        private readonly IAuthenticationService _authenticationService;
        private readonly IPssInvoiceHandler _handler;

        public PssInvoiceController(IAuthenticationService authenticationService, IHandler compHandler, IPssInvoiceHandler handler) : base(authenticationService, compHandler)
        {
            _compHandler = compHandler;
            _authenticationService = authenticationService;
            _handler = handler;
        }
        

        public virtual ActionResult GetInvoice(string BIN)
        {
            try
            {
                if (string.IsNullOrEmpty(BIN)) { throw new NoInvoicesMatchingTheParametersFoundException { }; }
                Logger.Information("Getting invoice " + BIN);
                string invoiceURL = _handler.GetInvoiceURL(BIN);
                return View(new InvoiceViewVM { URL = invoiceURL });
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
            }
            return View(new InvoiceViewVM { Error = true, ErrorMessage = ErrorLang.invoice404().ToString() });
        }



        /// <summary>
        /// Search by Invoice number
        /// </summary>
        public virtual ActionResult SearchByInvoiceNumber()
        {
            string message = string.Empty;
            bool hasError = false;
            string bin = null;

            try
            {
                if (TempData.ContainsKey("NoInvoice"))
                {
                    bin = TempData["NoInvoice"].ToString();
                    message = ErrorLang.invoice404().ToString();
                    hasError = true;
                    TempData = null;
                }
                TempData = null;
                UserDetailsModel userDetails = GetLoggedInUserDetails();
                return View(new SearchByBINVM { HeaderObj = HeaderFiller(userDetails), ErrorMessage = message, HasErrors = hasError, BIN = bin });
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Exception in SearchByInvoiceNumber get {0}", exception.Message));
                TempData = null;
                TempData.Add("Error", ErrorLang.genericexception().ToString());
            }
            return RedirectToRoute("P.SelectService");
        }




        /// <summary>
        /// Search by Invoice number post
        /// </summary>
        /// <param name="model"></param>
        [HttpPost, ActionName("SearchByInvoiceNumber")]
        public virtual ActionResult SearchByInvoiceNumber(SearchByBINVM model)
        {
            TempData = null;
            string errorMessage = string.Empty;
            string bin = string.Empty;

            try
            {
                if(model == null || string.IsNullOrEmpty(model.BIN))
                {
                    throw new NoInvoicesMatchingTheParametersFoundException();
                }
                else
                {
                    bin = model.BIN;
                    InvoiceGeneratedResponseExtn invoiceDetails = _handler.SearchForInvoiceForPaymentView(model.BIN);
                    if (invoiceDetails != null)
                    {
                        TempData.Add("InvoiceDetails", JsonConvert.SerializeObject(invoiceDetails));
                        return RedirectToRoute("P.Make.Payment", new { invoiceNumber = invoiceDetails.InvoiceNumber });
                    }
                }
                Logger.Error("Invoice not found " + model.BIN);
            }
            catch (NoInvoicesMatchingTheParametersFoundException exception)
            {
                Logger.Error(exception, string.Format("Exception in SearchByInvoiceNumber POST {0}", exception.Message));
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Exception in SearchByInvoiceNumber {0}", exception.Message));
                TempData = null;
                TempData.Add("Error", ErrorLang.genericexception().ToString());
                return RedirectToRoute("P.SelectService");
            }

            errorMessage = ErrorLang.invoice404().ToString();
            UserDetailsModel userDetails = GetLoggedInUserDetails();
            return View(new SearchByBINVM { HeaderObj = HeaderFiller(userDetails), HasErrors = true, ErrorMessage = errorMessage, BIN = bin });
        }


    }
}