using Orchard;
using Orchard.Logging;
using Orchard.Security;
using Orchard.Users.Events;
using Orchard.Users.Services;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Module.Web.Controllers;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.OSGOF.Web.Controllers.Handlers.Contracts;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.OSGOF.Web.Controllers
{
    public class CollectionController : BaseCollectionController
    {
        private readonly IOSGOFCollectionHandler _handler;
        private readonly IOrchardServices _orchardServices;

        private readonly IUserService _userService;
        private readonly IMembershipService _membershipService;
        private readonly ICBSUserManager<CBSUser> _cbsUserService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IMembershipValidationService _membershipValidationService;
        private readonly IUserEventHandler _userEventHandler;
        private readonly ICellSiteHandler _siteOperatorHandler;


        public CollectionController(IOSGOFCollectionHandler handler, IOrchardServices orchardServices, IUserService userService, IMembershipService membershipService, ICBSUserManager<CBSUser> cbsUserService, IAuthenticationService authenticationService, IMembershipValidationService membershipValidationService, IUserEventHandler userEventHandler, ICellSiteHandler siteOperatorHandler) : base(handler, orchardServices, userService, membershipService, cbsUserService, authenticationService, membershipValidationService, userEventHandler)
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
            _siteOperatorHandler = siteOperatorHandler;
        }


        /// <summary>
        /// Perform other form validtion for user registeration
        /// <para>For OSGOF the TIN and the email are required fields</para>
        /// </summary>
        /// <param name="model"></param>
        /// <exception cref="DirtyFormDataException"></exception>
        protected override void DoOtherFormValidations(RegisterCBSUserObj model)
        {
            if (string.IsNullOrEmpty(model.RegisterCBSUserModel.TIN)) { this.ModelState.AddModelError("RegisterCBSUserModel.TIN", "TIN is required."); throw new DirtyFormDataException(); }
            if (string.IsNullOrEmpty(model.RegisterCBSUserModel.Email)) { this.ModelState.AddModelError("RegisterCBSUserModel.Email", "Add a valid email is required."); throw new DirtyFormDataException(); }
        }       
    }
}