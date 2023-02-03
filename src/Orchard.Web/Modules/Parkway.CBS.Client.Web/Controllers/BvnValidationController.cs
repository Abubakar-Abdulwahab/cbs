using Orchard;
using Orchard.Logging;
using Orchard.Security;
using Orchard.Users.Events;
using Orchard.Users.Services;
using Parkway.CBS.Client.Web.Controllers.Handlers.Contracts;
using Parkway.CBS.Client.Web.ViewModels;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Module.Web.Controllers;
using Parkway.CBS.Module.Web.Controllers.Handlers.Contracts;
using Parkway.CBS.Module.Web.Middleware.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Parkway.CBS.Client.Web.Controllers
{
    [AnonymousOnly]
    public class BvnValidationController : BaseController
    {
        private readonly IModuleCollectionHandler _handler;
        private readonly IOrchardServices _orchardServices;

        private readonly IUserService _userService;
        private readonly IMembershipService _membershipService;
        private readonly ICBSUserManager<CBSUser> _cbsUserService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IMembershipValidationService _membershipValidationService;
        private readonly IUserEventHandler _userEventHandler;
        private readonly ICoreStateAndLGA _coreStateLGAService;
        private readonly IBvnValidationHandler _bvnHandler;

        public BvnValidationController(IModuleCollectionHandler handler, IOrchardServices orchardServices, IUserService userService, IMembershipService membershipService, ICBSUserManager<CBSUser> cbsUserService, IAuthenticationService authenticationService, IMembershipValidationService membershipValidationService, IUserEventHandler userEventHandler, ICoreStateAndLGA coreStateLGAService, IBvnValidationHandler bvnHandler) : base(orchardServices, userService, membershipService, cbsUserService, authenticationService, membershipValidationService, userEventHandler, handler)
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
            _coreStateLGAService = coreStateLGAService;
            _bvnHandler = bvnHandler;
        }

        // GET: BvnValidation
        [BrowserHeaderFilter]
        public ActionResult BvnValidation()
        {
            string message = string.Empty;
            string errorMessage = string.Empty;
            bool hasError = false;
            ValidateBvnVM model = new ValidateBvnVM { };
            try
            {
                if (TempData.ContainsKey("Message"))
                {
                    message = TempData["Message"].ToString();
                    TempData.Remove("Message");
                }
                if (TempData.ContainsKey("Error"))
                {
                    hasError = true;
                    errorMessage = TempData["Error"].ToString();
                    TempData.Remove("Error");
                }
                model = new ValidateBvnVM { StateLGAs = _coreStateLGAService.GetStates(), HeaderObj = HeaderFiller(null), RegisterCBSUserModel = new RegisterCBSUserModel { }, Error = hasError, ErrorMessage = errorMessage, Message = message };
                return View("BvnValidation/ValidateBvn", model);

            }
            catch (Exception exception)
            {
                Logger.Error(exception,exception.Message);
            }

            return View("BvnValidation/ValidateBvn", model);
        }

        [HttpPost]
        public ActionResult BvnValidation(ValidateBvnVM model)
        {
            string errorMessage = string.Empty;
            try
            {
                bool modelIsValid = this.TryValidateModel(model.RegisterCBSUserModel);
                if (!modelIsValid) { throw new DirtyFormDataException(); }
                //validate phone number
                string sPhoneNumber = model.RegisterCBSUserModel.PhoneNumber;
                sPhoneNumber = sPhoneNumber.Replace(" ", string.Empty);
                sPhoneNumber = sPhoneNumber.Replace("-", string.Empty);
                long phoneNumber = 0;
                bool isANumber = long.TryParse(sPhoneNumber, out phoneNumber);
                if (!isANumber)
                { this.ModelState.AddModelError("RegisterCBSUserModel.PhoneNumber", "Add a valid mobile phone number."); throw new DirtyFormDataException(); }
                if(String.IsNullOrEmpty(model.RegisterCBSUserModel.BVN) || model.RegisterCBSUserModel.BVN.Length != 11)
                {
                    Logger.Error("No BVN Specified");
                    this.ModelState.AddModelError("RegisterCBSUserModel.BVN", "Add a bank verification number.");
                    throw new DirtyFormDataException();
                }
                long bvn = 0;
                if(!long.TryParse(model.RegisterCBSUserModel.BVN, out bvn)) { Logger.Error("Unable to parse BVN"); throw new DirtyFormDataException(); }

                RegisterUserResponse createdUser = _bvnHandler.TryRegisterCBSUser(this, model);
                TempData = null;
                TempData.Add("Message", $"Registration successful, your State TIN : {createdUser.TaxEntityVM.PayerId}");
                return RedirectToRouteX(Client.Web.RouteName.BvnValidation.ValidateBVN);
            }
            #region catch clauses
            catch (DirtyFormDataException exception)
            {
                Logger.Error(exception.Message, exception);
                errorMessage = ErrorLang.genericexception().ToString();
            }
            catch (NoCategoryFoundException exception)
            {
                Logger.Error(exception.Message, exception);
                errorMessage = ErrorLang.categorynotfound().ToString();
            }
            catch (CBSUserAlreadyExistsException)
            {
                errorMessage = ErrorLang.profilealreadyexists().ToString();
            }
            catch (PhoneNumberHasBeenTakenException)
            {
                errorMessage = ErrorLang.phonenumberalreadyexists().ToString();
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message, exception);
                errorMessage = ErrorLang.genericexception().ToString();
            }
            #endregion

            model.StateLGAs = _coreStateLGAService.GetStates();
            if (model.RegisterCBSUserModel.SelectedState != 0) { model.ListLGAs = model.StateLGAs.Where(x => x.Id == model.RegisterCBSUserModel.SelectedState).Single().LGAs.ToList(); }
            model.HeaderObj = HeaderFiller(null);
            model.Error = true;
            model.ErrorMessage = errorMessage;
            return View("BvnValidation/ValidateBvn", model);
        }
    }
}