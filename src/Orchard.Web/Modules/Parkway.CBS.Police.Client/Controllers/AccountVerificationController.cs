using System;
using System.Web.Mvc;
using Orchard.Logging;
using Orchard.Security;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Client.Middleware;
using Orchard.Users.Events;
using Parkway.CBS.Police.Core.DTO;

namespace Parkway.CBS.Police.Client.Controllers
{
    [CheckVerificationFilter(false)]
    public class AccountVerificationController : BaseController
    {
        private readonly IHandler _compHandler;
        private readonly IAuthenticationService _authenticationService;
        private readonly IAccountVerificationHandler _handler;
        private readonly IUserEventHandler _userEventHandler;
        private readonly IMembershipService _membershipService;

        public AccountVerificationController(IHandler compHandler, IAuthenticationService authenticationService, IAccountVerificationHandler handler, IUserEventHandler userEventHandler, IMembershipService membershipService)
            : base(authenticationService, compHandler)
        {
            _compHandler = compHandler;
            _authenticationService = authenticationService;
            _handler = handler;
            _userEventHandler = userEventHandler;
            _membershipService = membershipService;
        }


        /// <summary>
        /// Route Name: P.Resend.Verification.Code
        /// 
        /// </summary>
        /// <param name="token"></param>
        [HttpPost]
        public JsonResult ResendVerificationCode(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                {
                    return Json(new APIResponse { Error = true, ResponseObject = ErrorLang.custommessage("No token found.").ToString() });
                }
                if (_handler.TokenHasExpired(token))
                {
                    return Json(new APIResponse { Error = true, ResponseObject = new { Refresh = true, Message = Lang.tokenexpiredredirecting.ToString() } });
                }
                QueueAVerificationCodeResend(token);
                return Json(new APIResponse { ResponseObject = Lang.codehasbeenresent.ToString() });
            }
            catch (UserNotAuthorizedForThisActionException)
            {
                TempData = null;
                TempData.Add("Info", Lang.youraccounthasalreadybeenverified.ToString());
                return Json(new APIResponse { Error = true, ResponseObject = new { Refresh = true, Message = Lang.tokenexpiredredirecting.ToString() } });
            }
            catch (InvalidOperationException exception)
            {
                Logger.Error(exception, exception.Message + "token: " + token);
                return Json(new APIResponse { Error = true, ResponseObject = ErrorLang.exceededcoderetry().ToString() });
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
            }
            return Json(new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().ToString() });
        }



        /// <summary>
        /// Route name: P.Verify.Account
        /// </summary>
        /// <param name="token"></param>
        public ActionResult VerifyAccount(string token)
        {
            try
            {
                UserDetailsModel userDetails = CheckVerificationStatus();

                if (string.IsNullOrEmpty(token))
                {
                    TempData = null;
                    if (userDetails == null) { TempData.Add("Error", ErrorLang.pleaseregister().ToString()); }
                    return RedirectToRoute("P.Register.User");
                }
                //need to check that the token hasn't expired
                if (_handler.TokenHasExpired(token))
                {
                    try
                    {
                        TokenExpiryResponse response = _handler.GetTokenExpiryResponseToToken(token);
                        TempData = null;
                        TempData.Add("Info", Lang.yourprevcodeexpiredanewcodehasbeengeneratedforyou.ToString());
                        return RedirectToRoute("P.Verify.Account", new { token = SendVerificationCode(response.CBSUser, CBS.Core.Models.Enums.VerificationType.AccountVerification, response.RedirectObj) });
                    }
                    catch (Exception exception)
                    {
                        Logger.Error(exception, "Exception while trying to regenerate token after expiry " + token + " " + exception.Message);
                        TempData = null;
                        TempData.Add("Error", ErrorLang.notokenfound().ToString());
                        return RedirectToRoute("P.SelectService");
                    }
                }
                return View(new VerifyAccountModel { HeaderObj = HeaderFiller(userDetails), Token = token, FlashObj = GetTextFromTempData("Info", CBS.Core.Models.Enums.FlashType.Info, "Info!") });
            }
            catch (UserNotAuthorizedForThisActionException exception)
            {
                TempData = null;
                TempData.Add("Info", Lang.youraccounthasalreadybeenverified.ToString());
                Logger.Error(exception, string.Format("Exception in VerifyAccount {0}", exception.Message));
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Exception in VerifyAccount {0}", exception.Message));
                TempData = null;
                TempData.Add("Error", ErrorLang.genericexception().ToString());
            }
            return RedirectToRoute("P.SelectService");
        }

        

        [HttpPost]
        [ActionName("VerifyAccount")]
        public ActionResult VerifyAccountPost(string token, string code)
        {
            try
            {
                UserDetailsModel userDetails = CheckVerificationStatus();
                if (string.IsNullOrEmpty(token))
                {
                    TempData = null;
                    if (userDetails == null) { TempData.Add("Error", ErrorLang.pleaseregister().ToString()); }
                    return RedirectToRoute("P.Register.User");
                }
                ValidateVerReturnValue retValue = _handler.DoTokenValidation(token, code);
                //redirect to transit page
                //now let check for redirects
                if(retValue.RedirectObj != null)
                {
                    if(_authenticationService.GetAuthenticatedUser() == null)
                    {
                        string email = retValue.RedirectObj.RedirectObject.Email.ToString();
                        string password = retValue.RedirectObj.RedirectObject.Password.ToString();
                        IUser ouser = _membershipService.ValidateUser(email, password);
                        if (ouser == null)
                        {
                            throw new NoRecordFoundException();
                        }

                        userDetails = GetUserDetails(ouser.Id, false);
                        if (userDetails == null)
                        {
                            throw new NoRecordFoundException();
                        }

                        Logger.Information($"{email} is about to be signed in");
                        _userEventHandler.LoggingIn(email, password);
                        _authenticationService.SetAuthenticatedUserForRequest(ouser);
                        _authenticationService.SignIn(ouser, true);
                        _userEventHandler.LoggedIn(ouser);
                        Logger.Information($"{email} LoggedIn set");
                    }

                    PSServiceVM serviceVM = new PSServiceVM
                    {
                        CategoryId = (int)retValue.RedirectObj.RedirectObject.CategoryId,
                        ServiceId = (int)retValue.RedirectObj.RedirectObject.ServiceId,
                        ServiceType = (int)retValue.RedirectObj.RedirectObject.ServiceType,
                        ServicePrefix = retValue.RedirectObj.RedirectObject.ServicePrefix.ToString(),
                        ServiceName = retValue.RedirectObj.RedirectObject.ServiceName.ToString(),
                        ServiceNote = retValue.RedirectObj.RedirectObject.ServiceNote.ToString()
                    } ;

                    //build session for select service
                    StartRequestSession(serviceVM, (int)retValue.RedirectObj.RedirectObject.SubCategoryId, (int)retValue.RedirectObj.RedirectObject.SubSubCategoryId, (PSSUserRequestGenerationStage)retValue.RedirectObj.RedirectObject.Stage, new CBSUserVM { Id = retValue.CBSUserId, IsAdministrator = retValue.IsAdministrator, TaxEntity = new TaxEntityViewModel { Id = userDetails.TaxPayerProfileVM.Id } });

                    return View("VerificationTransitPage", new VerifyAccountModel { HeaderObj = HeaderFiller(userDetails), Redirecting = true, URL = Url.RouteUrl(retValue.RedirectObj.RouteName) });
                }
                return View("VerificationTransitPage", new VerifyAccountModel { HeaderObj = HeaderFiller(userDetails), Redirecting = true, URL = Url.RouteUrl("P.Sign.In") });
            }
            catch (UserNotAuthorizedForThisActionException)
            {
                TempData = null;
                TempData.Add("Info", Lang.youraccounthasalreadybeenverified.ToString());
            }
            catch (TimeoutException)
            {
                return RedirectToRoute("P.Verify.Account", new { token });
            }
            catch (NoRecordFoundException)
            {
                TempData = null;
                TempData.Add("Error", ErrorLang.nomatchingcodefound().ToString());
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Exception in VerifyAccountPost {0}", exception.Message));
                TempData = null;
                TempData.Add("Error", ErrorLang.genericexception().ToString());
            }
            return RedirectToRoute("P.SelectService");
        }      

    }
}