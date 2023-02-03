using Orchard;
using Orchard.Logging;
using Orchard.Security;
using Orchard.Themes;
using Orchard.Users.Events;
using Orchard.Users.Services;
using Parkway.CBS.Client.Web.Controllers.Handlers.Contracts;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Module.Web.Controllers;
using Parkway.CBS.Module.Web.Controllers.Handlers.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Parkway.CBS.Client.Web.Controllers
{
    public class RegisterBusinessController : BaseController
    {
        private readonly IRegisterBusinessHandler _handler;
        private readonly IOrchardServices _orchardServices;

        private readonly IUserService _userService;
        private readonly IMembershipService _membershipService;
        private readonly ICBSUserManager<CBSUser> _cbsUserService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IMembershipValidationService _membershipValidationService;
        private readonly IUserEventHandler _userEventHandler;
        private readonly ICoreStateAndLGA _coreStateLGAService;

        public RegisterBusinessController(IRegisterBusinessHandler handler, IOrchardServices orchardServices, IUserService userService, IMembershipService membershipService, ICBSUserManager<CBSUser> cbsUserService, IAuthenticationService authenticationService, IMembershipValidationService membershipValidationService, IUserEventHandler userEventHandler, ICoreStateAndLGA coreStateLGAService) : base(orchardServices, userService, membershipService, cbsUserService, authenticationService, membershipValidationService, userEventHandler, handler)
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
        }


        // GET: RegisterBusiness
        [Themed]
        [HttpGet]
        public ActionResult RegisterBusiness()
        {
            try
            {
                return View("RegisterBusiness", new RegisterBusinessObj { RegisterBusinessModel = new RegisterBusinessModel { }, HeaderObj = GetHeaderObj(), StateLGAs = _coreStateLGAService.GetStates() });
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
            }
            return RedirectToRouteX("C.HomePage");
        }

        [HttpPost]
        public ActionResult RegisterBusiness(RegisterBusinessObj userInput)
        {
            string errorMessage = string.Empty;
            try
            {
                bool modelIsValid = this.TryValidateModel(userInput.RegisterBusinessModel);
                if (!modelIsValid) { throw new DirtyFormDataException(); }
                //Validate Phone number
                string sPhoneNumber = userInput.RegisterBusinessModel.PhoneNumber;
                sPhoneNumber = sPhoneNumber.Replace(" ", string.Empty);
                sPhoneNumber = sPhoneNumber.Replace("-", string.Empty);
                long phoneNumber = 0;
                bool isANumber = long.TryParse(sPhoneNumber, out phoneNumber);
                if (!isANumber)
                { this.ModelState.AddModelError("RegisterBusinessModel.PhoneNumber", "Add a valid mobile phone number."); throw new DirtyFormDataException(); }
                //validate contact person phone number if provided
                string sContactPersonPhoneNumber = userInput.RegisterBusinessModel.ContactPersonPhoneNumber;
                sContactPersonPhoneNumber = sContactPersonPhoneNumber.Replace(" ", string.Empty);
                sContactPersonPhoneNumber = sContactPersonPhoneNumber.Replace("-", string.Empty);
                long contactPersonPhoneNumber = 0;
                bool isANumberT = long.TryParse(sContactPersonPhoneNumber, out contactPersonPhoneNumber);
                if (!isANumberT)
                { this.ModelState.AddModelError("RegisterBusinessModel.ContactPersonPhoneNumber", "Add a valid mobile phone number"); throw new DirtyFormDataException(); }

                userInput.TaxPayerType = ((int)TaxEntityCategoryEnum.Corporate).ToString();

                _handler.TryRegisterBusiness(this, userInput);
                TempData = null;
                TempData.Add("Message", Lang.registrationsuccessful);
                TempData.Add("Username", userInput.RegisterBusinessModel.UserName);
                return RedirectToRouteX("C.SignIn");

            }
            catch (DirtyFormDataException exception)
            {
                Logger.Error(exception.Message, exception);
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

            userInput.HeaderObj = GetHeaderObj();
            userInput.StateLGAs = _coreStateLGAService.GetStates();
            userInput.ListLGAs = (userInput.RegisterBusinessModel.SelectedState != 0) ? userInput.StateLGAs.Where(x => x.Id == userInput.RegisterBusinessModel.SelectedState).First().LGAs.ToList() : null;
            return View(userInput);
        }
    }
}