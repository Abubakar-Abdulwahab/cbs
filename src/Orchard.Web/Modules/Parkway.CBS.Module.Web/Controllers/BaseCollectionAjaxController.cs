using Orchard;
using Orchard.Logging;
using Orchard.Security;
using Orchard.Users.Events;
using Orchard.Users.Services;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Module.Web.Controllers.Handlers.Contracts;
using Parkway.CBS.Module.Web.Middleware.Filters;
using System.Web.Mvc;

namespace Parkway.CBS.Module.Web.Controllers
{
    public class BaseCollectionAjaxController : BaseController
    {
        private readonly IModuleCollectionAjaxHandler _handler;
        private readonly IOrchardServices _orchardServices;

        private readonly IUserService _userService;
        private readonly IMembershipService _membershipService;
        private readonly ICBSUserManager<CBSUser> _cbsUserService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IMembershipValidationService _membershipValidationService;
        private readonly IUserEventHandler _userEventHandler;
        private readonly ICommonBaseHandler _commonBaseHandler;


        public BaseCollectionAjaxController(IModuleCollectionAjaxHandler handler, IOrchardServices orchardServices, IUserService userService, IMembershipService membershipService, ICBSUserManager<CBSUser> cbsUserService, IAuthenticationService authenticationService, IMembershipValidationService membershipValidationService, IUserEventHandler userEventHandler, ICommonBaseHandler commonBaseHandler) : base(orchardServices, userService, membershipService, cbsUserService, authenticationService, membershipValidationService, userEventHandler, commonBaseHandler)
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
            _commonBaseHandler = commonBaseHandler;
            //cellSites = new Dictionary<string, CellSitesDropdownBindingVM>();
        }

        [HttpPost, AnonymousOnly]
        /// <summary>
        /// Get tax profiles by category
        /// </summary>
        /// <param name="categoryId"></param>
        public virtual JsonResult GetTaxEntitiesByCategory(string categoryId)
        {
            Logger.Information("starting process payee");
            return Json(_handler.GetTaxProfilesByCategory(categoryId), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual JsonResult GetReferenceNumber(int invoiceId, string invoiceNumber, PaymentProvider provider = PaymentProvider.Bank3D)
        {
            Logger.Information($"About to get payment Reference Number for Invoice Id {invoiceId}, Invoice Id:::{invoiceId}, Provider:::{provider}");
            APIResponse response = _handler.GetPaymentReferenceNumber(invoiceId, invoiceNumber, provider);
            Logger.Information($"Gotten Payment Reference Number {response.ResponseObject} for Invoice Id {invoiceId}");

            return Json(response, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual JsonResult SendPaymentNotification(string paymentReference, PaymentProvider provider = PaymentProvider.Bank3D)
        {
            Logger.Information($"About to send payment notification for payment reference number {paymentReference}, payment reference:::{paymentReference}");
            return Json(_handler.SendNotification(paymentReference, provider), JsonRequestBehavior.AllowGet);
        }



        [HttpPost, AnonymousOnly]
        /// <summary>
        /// Get the LGAs of a state with the specified Id
        /// </summary>
        /// <param name="stateId">Id of the state</param>
        /// <returns>Json object filled with LGAs</returns>
        public virtual JsonResult GetLgasByStates(string stateId)
        {
            Logger.Information("retrieving state lgas");
            return Json(_handler.GetLgasByStates(stateId), JsonRequestBehavior.AllowGet);
        }
    }
}