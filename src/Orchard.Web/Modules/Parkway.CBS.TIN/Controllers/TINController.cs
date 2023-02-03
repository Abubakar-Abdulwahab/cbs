using Orchard;
using Orchard.Logging;
using Orchard.Security;
using Orchard.Users.Events;
using Orchard.Users.Services;
using Parkway.CBS.TIN.Controllers.Handlers.Contracts;
using Parkway.CBS.TIN.ViewModels;
using Parkway.CBS.Module.Web.Controllers.Handlers.Contracts;
using System;
using System.Web.Mvc;
using Orchard.Themes;
using Parkway.CBS.TIN.Middleware;

namespace Parkway.CBS.TIN.Controllers
{
    [Themed]
    [BrowserHeaderFilter]
    public class TINController : Controller
    {
        public ILogger Logger { get; set; }
        private readonly ITINHandler _tinHandler;
        private readonly IModuleCollectionHandler _handler;
        private readonly IOrchardServices _orchardServices;

        private readonly IUserService _userService;
        private readonly IMembershipService _membershipService;
        private readonly Core.Services.Contracts.ICBSUserManager<Core.Models.CBSUser> _cbsUserService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IMembershipValidationService _membershipValidationService;
        private readonly IUserEventHandler _userEventHandler;

        public TINController(ITINHandler tinHandler, IModuleCollectionHandler handler, IOrchardServices orchardServices, IUserService userService, IMembershipService membershipService, Core.Services.Contracts.ICBSUserManager<Core.Models.CBSUser> cbsUserService, IAuthenticationService authenticationService, IMembershipValidationService membershipValidationService, IUserEventHandler userEventHandler) 
        {
            _orchardServices = orchardServices;
            _userService = userService;
            _membershipService = membershipService;
            _cbsUserService = cbsUserService;
            _authenticationService = authenticationService;
            _membershipValidationService = membershipValidationService;
            _userEventHandler = userEventHandler;
            _tinHandler = tinHandler;
            Logger = NullLogger.Instance;
        }
        
        /// <summary>
        /// TIN Form
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult TIN()
        {
            //Publish("nil");
            var gender = new Core.Models.Enums.Gender();
            ViewData["Gender"] = gender;

            var user = GetLoggedInUserDetails();
            var headerObj = new Core.HelperModels.HeaderObj { };
            if (user != null && user.Entity != null) { headerObj.IsLoggedIn = true; headerObj.DisplayText = user.Name; }
            headerObj.ShowSignin = true;
            return View("TINForm", new TINFormViewModel { HeaderObj = headerObj });
        }

        /// <summary>
        /// Process and Save TIN Applicant Registration
        /// </summary>
        /// <param name="formData"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult TIN(TINFormViewModel formData)
        {
            try
            {
               var result = _tinHandler.TryRegisterTIN(formData);
                //var model = _tinService.TINRegistrationForm();
                //Publish("Successful");
                if (result == true)
                {
                    Session["TINFormRegistration"] = "Successful";
                    return RedirectToAction("GenerateReference");
                }
                Session["TINFormRegistration"] = "Failed";
                return RedirectToAction("GenerateReference");
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// Generate a reference number to identify each TIN Applicant
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GenerateReference()
        {
            try
            {
                var sessionValue = Session["TINFormRegistration"] as string;  
                if (sessionValue == "Failed")
                {
                    Logger.Information("TIN Registration Not Successful,Display Error Message");
                    return View("TINError");
                }
                Logger.Information("About to generate reference");
                var reference = string.Format("NIRS|{0}|{1}", DateTime.Now.ToString("yyyyMMdd"), _tinHandler.GetLastGeneratedTin().ToString().PadLeft(6));
                ViewBag.Reference = reference;
                return View("Reference");
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                return View("Reference");

            }
        }

        public ActionResult SaveApplicantTIN(long TINId, string TIN)
        {
            var result = _tinHandler.UpdateTIN(TINId, TIN);
            ViewBag.UpdateTINResult = "successful";

            return RedirectToAction("Index", "TINReport");
            //return View();
        }


        /// <summary>
        /// Get the details of the logged in user
        /// <para>Would return an initialized object at all time, so check if entity is null to validate if there is a user for this request</para>
        /// </summary>
        /// <returns>UserDetailsModel</returns>
        protected virtual Core.HelperModels.UserDetailsModel GetLoggedInUserDetails()
        {
            Logger.Information("Getting orchard work context");
            IUser currentUser = null;
            string name = string.Empty;
            Core.HelperModels.UserDetailsModel userDetails = new Core.HelperModels.UserDetailsModel();
            try
            {
                currentUser = _authenticationService.GetAuthenticatedUser();
                Logger.Information(string.Format("List of cookies {0}", string.Join(",", Request.Cookies.AllKeys)));

                if (currentUser == null)
                {
                    Logger.Information(string.Format("No get auth user found, trying work context, user is auth statuse {0}", Request.IsAuthenticated));
                    return userDetails;
                }
                //check if the current user is a CBS user

                Logger.Information(string.Format("Current orchard user for current user {0}", currentUser.UserName));
                userDetails = _cbsUserService.GetUserDetails(currentUser.Id);

                if (userDetails == null)
                {
                    //_authenticationService.SignOut();
                    Logger.Information("orchard user found but no CBS user");
                    //prehistoric setting, I think some methods call this function expecting the userDetails to be initialized, so checks for null
                    //are not done until TaxEntity is checked
                    return new Core.HelperModels.UserDetailsModel { };
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
            }
            return userDetails;
        }
    }
}