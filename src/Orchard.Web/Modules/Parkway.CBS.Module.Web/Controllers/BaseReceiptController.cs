using Orchard;
using Orchard.Logging;
using Orchard.Security;
using Orchard.Users.Events;
using Orchard.Users.Services;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Module.Web.Controllers.Handlers.Contracts;
using Parkway.CBS.Module.Web.ViewModels;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Parkway.CBS.Module.Web.Controllers
{
    public class BaseReceiptController : BaseController
    {
        private readonly IModuleReceiptHandler _handler;
        private readonly IOrchardServices _orchardServices;

        private readonly IUserService _userService;
        private readonly IMembershipService _membershipService;
        private readonly ICBSUserManager<CBSUser> _cbsUserService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IMembershipValidationService _membershipValidationService;
        private readonly IUserEventHandler _userEventHandler;



        public BaseReceiptController(IModuleReceiptHandler handler, IOrchardServices orchardServices, IUserService userService, IMembershipService membershipService, ICBSUserManager<CBSUser> cbsUserService, IAuthenticationService authenticationService, IMembershipValidationService membershipValidationService, IUserEventHandler userEventHandler) : base(orchardServices, userService, membershipService, cbsUserService, authenticationService, membershipValidationService, userEventHandler, handler)
        {
            _handler = handler;
            Logger = NullLogger.Instance;
            _orchardServices = orchardServices;
            _userService = userService;
            _membershipService = membershipService;
            _cbsUserService = cbsUserService;
            _authenticationService = authenticationService;
            _membershipValidationService = membershipValidationService;
            _userEventHandler = userEventHandler;
        }



        /// <summary>
        /// Route name: C.Invoice.ReceiptDetails
        /// URL: c/invoice/receipts/{invoiceNumber}
        /// </summary>
        /// <param name="invoiceNumber"></param>
        public virtual ActionResult InvoiceNumberReceipts(string invoiceNumber)
        {
            InvoiceGeneratedResponseExtn invoiceDetails = _handler.SearchForInvoiceForPaymentView(invoiceNumber);
            if (invoiceDetails == null)
            {
                Logger.Error("Invoice not found " + invoiceNumber);
                TempData = null;
                TempData.Add("NoInvoice", invoiceNumber);
                return RedirectToRouteX("BIN.Search");
            }

            UserDetailsModel user = GetLoggedInUserDetails();
            HeaderObj headerObj = new HeaderObj { ShowSignin = true };
            if (user != null && user.Entity != null) { headerObj.IsLoggedIn = true; headerObj.DisplayText = user.Name; }
            invoiceDetails.HeaderObj = headerObj;

            return View("Receipt/InvoiceNumberReceipts", invoiceDetails);
            //return View(invoiceDetails);
        }


    }
}