using System;
using System.Web.Mvc;
using Orchard.Logging;
using Orchard.Security;
using Orchard.Users.Events;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Client.Middleware;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;

namespace Parkway.CBS.Police.Client.Controllers
{
    [CheckVerificationFilter(false)]
    public class LoginController : BaseController
    {
        private readonly IHandler _compHandler;
        private readonly IAuthenticationService _authenticationService;
        private readonly IMembershipService _membershipService;
        private readonly IUserEventHandler _userEventHandler;


        public LoginController(IHandler compHandler, IAuthenticationService authenticationService, IMembershipService membershipService, IUserEventHandler userEventHandler)
            : base(authenticationService, compHandler)
        {
            _compHandler = compHandler;
            _authenticationService = authenticationService;
            _membershipService = membershipService;
            _userEventHandler = userEventHandler;
        }


        /// <summary>
        /// Route Name: P.Sign.In
        /// </summary>
        /// <param name="r"></param>
        [PSSAnonymous]
        public ActionResult SignIn(string r)
        {
            try
            {
                FlashObj flashObj = null;
                try
                {
                    flashObj = GetTextFromTempData("Info", FlashType.Info, "Info!");
                }
                catch (Exception exception)
                { Logger.Error(exception, string.Format("Exception getting removing PSSRequestStage from session value {0}", exception.Message)); }

                TempData = null;
                UserDetailsModel userDetails = GetLoggedInUserDetails(false);
                if (userDetails == null)
                {
                    return View(new LogInObj { HeaderObj = new HeaderObj { }, FlashObj = flashObj, ReturnURL = string.IsNullOrEmpty(r) ? "" : r });
                }
            }
            catch (Exception exception)
            {
                TempData = null;
                Logger.Error(exception, string.Format("Exception in SignIn {0}", exception.Message));
                TempData.Add("Error", ErrorLang.genericexception().ToString());
            }
            return RedirectToRoute("P.SelectService");
        }



        /// <summary>
        /// Route Name: P.Sign.In
        /// </summary>
        /// <param name="model"></param>
        /// <param name="r"></param>
        [HttpPost]
        [PSSAnonymous]
        public ActionResult SignIn(LogInObj model, string r)
        {
            try
            {
                TempData = null;
                Logger.Information(string.Format("{0} is signing in", model.Email));
                //validate email
                if (string.IsNullOrEmpty(model.Email)) { this.ModelState.AddModelError("Email", "A valid email is required."); throw new DirtyFormDataException(); }
                bool parsed = model.Email.Contains("@");
                if (!parsed) { this.ModelState.AddModelError("Email", "A valid email address."); throw new DirtyFormDataException(); }
                parsed = !model.Email.Contains(" ");
                if (!parsed) { this.ModelState.AddModelError("Email", "A valid email address."); throw new DirtyFormDataException(); }

                if (string.IsNullOrEmpty(model.Password))
                {
                    this.ModelState.AddModelError("Password", "Password field is required."); throw new DirtyFormDataException();
                }

                IUser ouser = _membershipService.ValidateUser(model.Email, model.Password);
                if (ouser == null)
                {
                    return View(new LogInObj { FlashObj = new FlashObj { FlashType = FlashType.Error, Message = "Invalid login credentials", MessageTitle = "Error!"}, HeaderObj = new HeaderObj { }, Email = model.Email, ReturnURL = string.IsNullOrEmpty(r) ? "" : r });
                }

                if (GetUserDetails(ouser.Id, false) == null)
                {
                    return View(new LogInObj { HeaderObj = new HeaderObj { }, FlashObj = new FlashObj { FlashType = FlashType.Error, Message =  "Invalid login credentials" , MessageTitle = "Error!"}, ReturnURL = string.IsNullOrEmpty(r) ? "" : r, Email = model.Email });
                }

                Logger.Information(string.Format("{0} is about to be signed in", model.Email));
                _userEventHandler.LoggingIn(model.Email, model.Password);
                _authenticationService.SetAuthenticatedUserForRequest(ouser);
                _authenticationService.SignIn(ouser, true);
                _userEventHandler.LoggedIn(ouser);
                Logger.Information(string.Format("{0} LoggedIn set", model.Email));
            }
            catch (DirtyFormDataException)
            {
                return View(new LogInObj { HeaderObj = new HeaderObj { }, ReturnURL = string.IsNullOrEmpty(r) ? "" : r, Email = model.Email });
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Exception in SignIn {0}", exception.Message));
                TempData.Add("Error", ErrorLang.genericexception().ToString());
            }
            return RedirectToRoute("P.SelectService");
        }

    }
}