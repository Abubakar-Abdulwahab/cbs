using Orchard;
using Orchard.Logging;
using Orchard.Security;
using Orchard.Users.Events;
using Orchard.Users.Services;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Module.Web.Controllers;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Module.Web.Controllers.Handlers.Contracts;
using Orchard.Environment.Extensions;
using Orchard.Exceptions;
using System;
using Parkway.CBS.Module.Web.Middleware.Filters;
using System.Web.Mvc;
using Parkway.CBS.Core.Utilities;
using System.Linq;
using Parkway.CBS.Core.Models.Enums;

namespace Parkway.CBS.Payment.ClientWeb.Controllers
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

    }
}