using Orchard;
using Orchard.Security;
using Orchard.Logging;
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
using System.Web.Helpers;
using Parkway.CBS.Core.HelperModels;
using System.Web.Mvc;
using Parkway.CBS.Client.Web.Controllers.Handlers.Contracts;

namespace Parkway.CBS.Client.Web.Controllers
{
    public class AjaxValidateBvnController : BaseCollectionAjaxController
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
        private readonly IBvnValidationHandler _bvnHandler;


        public AjaxValidateBvnController(IModuleCollectionAjaxHandler handler, IOrchardServices orchardServices, IUserService userService, IMembershipService membershipService, ICBSUserManager<CBSUser> cbsUserService, IAuthenticationService authenticationService, IMembershipValidationService membershipValidationService, IUserEventHandler userEventHandler, IModuleCollectionHandler collectionHandler, ICommonBaseHandler commonBaseHandler, IBvnValidationHandler bvnHandler) : base(handler, orchardServices, userService, membershipService, cbsUserService, authenticationService, membershipValidationService, userEventHandler, commonBaseHandler)
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
            _bvnHandler = bvnHandler;
        }





        public JsonResult ValidateBvn(string bvn)
        {
            try
            {
                return Json(_bvnHandler.CheckIfBvnExists(bvn));
            }
            catch (Exception exception)
            {
                Logger.Error(exception,exception.Message);
            }

            return Json(new APIResponse { ResponseObject = null, Error = true });

        }
    }
}