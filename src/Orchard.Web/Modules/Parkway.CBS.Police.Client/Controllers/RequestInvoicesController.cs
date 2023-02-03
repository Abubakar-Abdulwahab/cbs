using Orchard.Logging;
using Orchard.Security;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Parkway.CBS.Police.Client.Controllers
{
    public class RequestInvoicesController : BaseController
    {
        private readonly IHandler _compHandler;
        private readonly IAuthenticationService _authenticationService;
        private readonly IRequestListHandler _policeRequestHandler;

        public RequestInvoicesController(IHandler compHandler, IAuthenticationService authenticationService, IRequestListHandler policeRequestHandler) : base(authenticationService, compHandler)
        {
            _compHandler = compHandler;
            _authenticationService = authenticationService;
            _policeRequestHandler = policeRequestHandler;
        }


        // GET: RequestInvoices
        public ActionResult RequestInvoices(string fileNumber)
        {
            try
            {
                if(!string.IsNullOrEmpty(fileNumber))
                {
                    PSSRequestInvoiceVM model = _policeRequestHandler.GetInvoicesForRequest(fileNumber);
                    model.HeaderObj = HeaderFiller(GetLoggedInUserDetails());
                    return View(model);
                }
                else { throw new Exception("Invalid File Ref Number"); }
            }
            catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
            }
            TempData.Add("Error", ErrorLang.genericexception().ToString());
            return RedirectToRoute(RouteName.SelectService.ShowSelectService);
        }


        public ActionResult GetInvoice(string bin)
        {
            try
            {
                Logger.Information("Getting invoice " + bin);
                string invoiceURL = _compHandler.GetInvoiceURL(bin);
                return View(new InvoiceViewVM { URL = invoiceURL });
            }catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
            }
            return View(new InvoiceViewVM { Error = true, ErrorMessage = ErrorLang.invoice404().ToString() });
        }
    }
}