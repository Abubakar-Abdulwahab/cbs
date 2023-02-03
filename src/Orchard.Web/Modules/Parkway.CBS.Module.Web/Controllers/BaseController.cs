using System;
using Orchard;
using Orchard.Themes;
using System.Web.Mvc;
using Orchard.Logging;
using Orchard.Security;
using Orchard.Users.Events;
using Orchard.Users.Services;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Module.Web.ViewModels;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Module.Web.Controllers.Handlers.Contracts;
using Parkway.CBS.Core.Utilities;
using Newtonsoft.Json;
using System.Collections.Generic;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Module.Web.Controllers.CommonHandlers.HelperHandlers.Contracts;

namespace Parkway.CBS.Module.Web.Controllers
{
    [Themed]
    public abstract class BaseController : Controller
    {
        public ILogger Logger { get; set; }
        private readonly IOrchardServices _orchardServices;

        private readonly IUserService _userService;
        private readonly IMembershipService _membershipService;
        private readonly ICBSUserManager<CBSUser> _cbsUserService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IMembershipValidationService _membershipValidationService;
        private readonly IUserEventHandler _userEventHandler;
        private readonly ICommonBaseHandler _commonHandler;
        

        public BaseController(IOrchardServices orchardServices, IUserService userService, IMembershipService membershipService, ICBSUserManager<CBSUser> cbsUserService, IAuthenticationService authenticationService, IMembershipValidationService membershipValidationService, IUserEventHandler userEventHandler, ICommonBaseHandler handler)
        {
            _orchardServices = orchardServices;
            Logger = NullLogger.Instance;
            _authenticationService = authenticationService;
            _userService = userService;
            _membershipService = membershipService;
            _membershipValidationService = membershipValidationService;
            _userEventHandler = userEventHandler;
            _cbsUserService = cbsUserService;
            _commonHandler = handler;
        }


        /// <summary>
        /// Add validations errors to the controller model
        /// </summary>
        /// <typeparam name="C">Controller</typeparam>
        /// <param name="callback">Web controller model</param>
        /// <param name="errors">List{ErrorModel}</param>
        /// <returns>T</returns>
        /// <exception cref="DirtyFormDataException"></exception>
        protected virtual void AddValidationErrorsToCallback<C>(C callback, List<ErrorModel> errors) where C : Controller
        {
            if (errors.Count > 0)
            {
                foreach (var item in errors) { callback.ModelState.AddModelError(item.FieldName, item.ErrorMessage.ToString()); }
                throw new DirtyFormDataException();
            }
        }


        /// <summary>
        /// Check if the stages match
        /// </summary>
        /// <param name="expectedStage"></param>
        /// <param name="actualStage"></param>
        /// <returns>bool</returns>
        protected virtual bool IsStageCorrect(InvoiceGenerationStage expectedStage, InvoiceGenerationStage actualStage)
        {
            return expectedStage == actualStage;
        }


        /// <summary>
        /// Get deserialized obj that has the stage model
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <returns>GenerateInvoiceStepsModel</returns>
        /// <exception cref="Exception"></exception>
        protected virtual GenerateInvoiceStepsModel GetDeserializedSessionObj(ref string errorMessage)
        {
            try { return JsonConvert.DeserializeObject<GenerateInvoiceStepsModel>(System.Web.HttpContext.Current.Session["InvoiceGenerationStage"].ToString()); }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Exception getting the deserializing the session value {0}", exception.Message));
                errorMessage = "Your session could not be continued. Please fill in your details and proceed to start new session.";
                throw new Exception(errorMessage);
            }
        }


        /// <summary>
        /// Check if the user claim is authentic
        /// </summary>
        /// <param name="claimToken"></param>
        /// <param name="requestToken"></param>
        /// <returns>TaxEntity | null</returns>
        protected virtual UserDetailsModel ValidateClaim(string claimToken, string requestToken)
        {
            UserClaim claim = ValidateClaims(claimToken, requestToken);
            if (claim == null) { return null; }
            //get the user that has this username
            IUser user = _membershipService.GetUser(claim.UserName);
            if (user == null) { return null; }

            Logger.Information("Getting current user");
            UserDetailsModel userDetailsModel = _cbsUserService.GetUserDetails(user.Id);
            if (userDetailsModel.CBSUser == null || userDetailsModel.Entity == null) { return null; }

            var loggedInUser = _authenticationService.GetAuthenticatedUser();
            if (loggedInUser != null) { return userDetailsModel; }

            _authenticationService.SetAuthenticatedUserForRequest(user);
            _authenticationService.SignIn(user, true);
            _userEventHandler.LoggedIn(user);

            return userDetailsModel;
        }


        /// <summary>
        /// Fill header obj
        /// <para>will always return an instance of the return object</para>
        /// </summary>
        /// <param name="userDetails"></param>
        /// <returns>HeaderObj</returns>
        protected virtual HeaderObj HeaderFiller(UserDetailsModel userDetails)
        {
            HeaderObj headerObj = new HeaderObj { };
            if (userDetails == null) return headerObj;
            headerObj.IsLoggedIn = true;
            headerObj.ShowSignin = true;
            headerObj.DisplayText = userDetails.Name;
            return headerObj;
        }


        /// <summary>
        /// Get header object
        /// </summary>
        /// <returns></returns>
        protected virtual HeaderObj GetHeaderObj()
        {
            UserDetailsModel userDetails = GetLoggedInUserDetails();
            HeaderObj obj = new HeaderObj { ShowSignin = true };
            if (userDetails != null && userDetails.Entity != null) { obj.IsLoggedIn = true; obj.DisplayText = userDetails.Name; }
            else { obj.IsLoggedIn = false; }
            return obj;
        }

        /// <summary>
        /// Validate that this claim is authentic
        /// </summary>
        /// <param name="claimToken"></param>
        /// <param name="requestToken"></param>
        /// <returns>UserClaim | null</returns>
        public virtual UserClaim ValidateClaims(string claimToken, string requestToken)
        {
            try
            {
                Logger.Debug(string.Format("Validating claim {0} {1}", claimToken, requestToken));
                string claimJson = Util.LetsDecrypt(claimToken, requestToken + System.Configuration.ConfigurationManager.AppSettings["EncryptionSecret"]);
                Logger.Debug("Decrypted claim " + claimJson);
                UserClaim claim = JsonConvert.DeserializeObject<UserClaim>(claimJson);
                if (claim == null) { Logger.Error("Claim returned null"); return null; }
                //get time to live in seconds
                DateTime createDate = claim.TTL;
                if (DateTime.Now.ToLocalTime() < claim.TTL) { return claim; }
                Logger.Error("Claim has expired");
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
            }
            return null;
        }


        


        /// <summary>
        /// Get the details of the logged in user
        /// <para>Would return an initialized object at all time, so check if entity is null to validate if there is a user for this request</para>
        /// </summary>
        /// <returns>UserDetailsModel</returns>
        protected virtual UserDetailsModel GetLoggedInUserDetails()
        {
            Logger.Information("Getting orchard work context");
            IUser currentUser = null;
            string name = string.Empty;
            UserDetailsModel userDetails = new UserDetailsModel();
            try
            {
                currentUser = _authenticationService.GetAuthenticatedUser();

                if (currentUser == null)
                {
                    return userDetails;
                }
                //check if the current user is a CBS user

                Logger.Information(string.Format("Current orchard user for current user {0}", currentUser.UserName));
                userDetails = _cbsUserService.GetUserDetails(currentUser.Id);

                if (userDetails == null)
                {
                    _authenticationService.SignOut();
                    //Logger.Information("orchard user found but no CBS user");
                    //prehistoric setting, I think some methods call this function expecting the userDetails to be initialized, so checks for null
                    //are not done until TaxEntity is checked
                    return new UserDetailsModel { };
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
            }
            return userDetails;
        }


        /// <summary>
        /// Remove invoice generation model from session memory
        /// </summary>
        protected virtual void EmptyInvoiceGenerationSession()
        {
            if (Session["InvoiceGenerationStage"] != null)
            {
                try
                { Session.Remove("InvoiceGenerationStage"); }
                catch (Exception exception)
                { Logger.Error(exception, string.Format("Exception getting the deserializing the session value {0}", exception.Message)); throw; }
            }
        }


        //
        // Summary:
        //     Redirects to the specified route using the route name and route dictionary.
        //
        // Parameters:
        //   routeName:
        //     The name of the route.
        //
        //   routeValues:
        //     The parameters for a route.
        //
        // Returns:
        //     The redirect-to-route result object.
        protected internal virtual RedirectToRouteResult RedirectToRouteX(string routeName, object routeValues = null)
        {
            //first we get the area this request is bound to
            object area = string.Empty;
            bool hasArea = HttpContext.Request.RequestContext.RouteData.Values.TryGetValue("area", out area);
            if (hasArea && !string.IsNullOrEmpty((string)area))
            {
                //if the area is found, we need to check if the said area has a prefix 
                string prefix = Util.GetRouteNamePrefix((string)area);
                routeName = prefix + routeName;
            }
            //Logger.Information(string.Format("RedirectToRouteX routeName : {0}", routeName));
            return RedirectToRoute(routeName, routeValues);
        }



    }
}