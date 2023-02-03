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
    public class TaxProfileController : BaseTaxProfileController
    {
        private readonly ITaxProfileHandler _handler;
        private readonly IOrchardServices _orchardServices;

        private readonly IUserService _userService;
        private readonly IMembershipService _membershipService;
        private readonly ICBSUserManager<CBSUser> _cbsUserService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IMembershipValidationService _membershipValidationService;
        private readonly IUserEventHandler _userEventHandler;


        public TaxProfileController(IOrchardServices orchardServices, IUserService userService, IMembershipService membershipService, ICBSUserManager<CBSUser> cbsUserService, IAuthenticationService authenticationService, IMembershipValidationService membershipValidationService, IUserEventHandler userEventHandler, ITaxProfileHandler handler) : base(orchardServices, userService, membershipService, cbsUserService, authenticationService, membershipValidationService, userEventHandler, handler)
        {
            _orchardServices = orchardServices;
            Logger = NullLogger.Instance;
            _handler = handler;
            _authenticationService = authenticationService;
            _userService = userService;
            _membershipService = membershipService;
            _membershipValidationService = membershipValidationService;
            _userEventHandler = userEventHandler;
            _cbsUserService = cbsUserService;
        }

    }
}