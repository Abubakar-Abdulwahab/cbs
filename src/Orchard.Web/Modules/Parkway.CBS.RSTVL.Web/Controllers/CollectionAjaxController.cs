using Orchard;
using Orchard.Logging;
using Orchard.Security;
using Orchard.Themes;
using Orchard.Users.Events;
using Orchard.Users.Services;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Module.Web.Controllers;
using Parkway.CBS.Module.Web.Controllers.Handlers.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Parkway.CBS.RSTVL.Web.Controllers
{
    public class CollectionAjaxController : BaseCollectionAjaxController
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
        private readonly IModuleCollectionHandler _collectionHandler;


        public CollectionAjaxController(IModuleCollectionAjaxHandler handler, IOrchardServices orchardServices, IUserService userService, IMembershipService membershipService, ICBSUserManager<CBSUser> cbsUserService, IAuthenticationService authenticationService, IMembershipValidationService membershipValidationService, IUserEventHandler userEventHandler, IModuleCollectionHandler collectionHandler, ICommonBaseHandler commonBaseHandler) : base(handler, orchardServices, userService, membershipService, cbsUserService, authenticationService, membershipValidationService, userEventHandler, commonBaseHandler)
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
            _collectionHandler = collectionHandler;
        }


        /// <summary>
        /// This is use to get the list of cell sites for operator page display
        /// </summary>
        /// <param name="scheduleRef"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public virtual JsonResult PaymentListMoveRight(string datefilter, long operatorId, int page)
        {
            Logger.Information(string.Format("getting page data for operator Id - {0} page - {1}", operatorId, page));
            return Json(_collectionHandler.GetPagedPaymentList(operatorId, page, datefilter), JsonRequestBehavior.AllowGet);
        }

    }
}