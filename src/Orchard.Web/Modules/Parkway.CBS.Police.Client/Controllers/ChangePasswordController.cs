using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Orchard;
using Orchard.Logging;
using Orchard.Security;
using Orchard.Users.Events;
using Orchard.Users.Services;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Client.Middleware;

namespace Parkway.CBS.Police.Client.Controllers
{
    [PSSAuthorized]
    public class ChangePasswordController : BaseController
    {
        private readonly IHandler _compHandler;
        private readonly IAuthenticationService _authenticationService;
        private readonly IMembershipService _membershipService;
        private readonly IUserEventHandler _userEventHandler;


        public ChangePasswordController(IHandler compHandler, IAuthenticationService authenticationService, IMembershipService membershipService, IUserEventHandler userEventHandler) : base(authenticationService, compHandler)
        {
            _compHandler = compHandler;
            _authenticationService = authenticationService;
            _membershipService = membershipService;
            _userEventHandler = userEventHandler;
        }



        /// <summary>
        /// Route Name: P.ChangePassword
        /// </summary>
        public ActionResult ChangePassword()
        {
            FlashObj flashObj = null;
            try
            {
                flashObj = GetTextFromTempData("ChangePasswordInfo", FlashType.Info, "Info");
            }
            catch (Exception exception)
            { Logger.Error(exception, string.Format("Exception getting removing PSSRequestStage from session value {0}", exception.Message)); }

            return View("ChangePassword/ChangePassword", new ChangePasswordModel { FlashObj = flashObj, HeaderObj = HeaderFiller(GetLoggedInUserDetails()) });
        }



        /// <summary>
        /// Change password post
        /// </summary>
        /// <param name="userinput"></param>
        [HttpPost, ActionName("ChangePassword")]
        public ActionResult ChangePassword(ChangePasswordModel userinput)
        {
            List<ErrorModel> errors = new List<ErrorModel> { };
            UserDetailsModel userDetailsModel = null;
            try
            {
                userDetailsModel = GetLoggedInUserDetails();
                if (!this.ModelState.IsValid) { throw new DirtyFormDataException(); }
                IUser validated = _membershipService.ValidateUser(userDetailsModel.TaxPayerProfileVM.Email, userinput.OldPassword);

                if(validated != null)
                {
                    _membershipService.SetPassword(validated, userinput.NewPassword);
                    _userEventHandler.ChangedPassword(validated);
                    TempData = null;
                    TempData.Add("ChangePasswordInfo", Lang.passwordchangedsuccessfully.ToString());
                    return ChangePassword();
                }
                else
                {
                    errors.Add(new ErrorModel { ErrorMessage = "Your old password does not match", FieldName = "OldPassword" });
                    throw new DirtyFormDataException { };
                }
            }
            catch (DirtyFormDataException exception)
            {
                if (errors.Count > 0)
                {
                    foreach (var item in errors)
                    { this.ModelState.AddModelError(item.FieldName, item.ErrorMessage.ToString()); }
                }
                Logger.Error(exception.Message, exception);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
            return View("ChangePassword/ChangePassword", new ChangePasswordModel { HeaderObj = HeaderFiller(userDetailsModel) });
        }
    }
}