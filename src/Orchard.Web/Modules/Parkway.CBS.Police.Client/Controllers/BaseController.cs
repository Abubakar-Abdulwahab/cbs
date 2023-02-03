using System;
using Orchard.Themes;
using System.Web.Mvc;
using Newtonsoft.Json;
using Orchard.Logging;
using Orchard.Security;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.VM;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Client.Middleware;
using Parkway.CBS.Core.Exceptions;
using Orchard.Environment.Extensions;
using Parkway.CBS.Police.Core.DTO;

namespace Parkway.CBS.Police.Client.Controllers
{
    [OrchardSuppressDependency("Orchard.Exceptions.Filters.UnhandledExceptionFilter")]
    [OutputCacheAttribute(Duration = 0)]
    [CheckVerificationFilter(true)]
    [Themed]
    public abstract class BaseController : Controller
    {
        public ILogger Logger { get; set; }
        private readonly IHandler _handler;
        private readonly IAuthenticationService _authenticationService;

        public BaseController(IAuthenticationService authenticationService, IHandler handler)
        {
            Logger = NullLogger.Instance;
            _authenticationService = authenticationService;
            _handler = handler;
        }


        /// <summary>
        /// Get text from temp data for flash obj
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="flashType"></param>
        /// <returns>FlashObj</returns>
        public FlashObj GetTextFromTempData(string keyName = "Error", CBS.Core.Models.Enums.FlashType flashType = CBS.Core.Models.Enums.FlashType.Error, string headerText = "Error!" )
        {
            FlashObj flashObj = null;
            try
            {
                if (TempData.ContainsKey(keyName))
                {
                    flashObj = new FlashObj { MessageTitle = headerText, Message = TempData[keyName].ToString(), FlashType = flashType };
                    TempData.Remove(keyName);
                }
            }
            catch (Exception exception) { Logger.Error(exception, "Error getting error value from temp data " + exception.Message); }
            TempData = null;
            return flashObj;
        }


        /// <summary>
        /// Get token for verification for this cbs user Id
        /// </summary>
        /// <param name="cbsUser"></param>
        /// <param name="verificationType">Enums.VerificationType</param>
        /// <param name="redirectObj">if a redirect is needed pass this param</param>
        /// <returns>string</returns>
        public string SendVerificationCode(CBSUserVM cbsUser, CBS.Core.Models.Enums.VerificationType verificationType, RedirectReturnObject redirectObj = null)
        {
            return _handler.ProviderVerificationToken(cbsUser, verificationType, redirectObj);
        }


        /// <summary>
        /// Resend verification token
        /// </summary>
        /// <param name="token"></param>
        /// <exception cref="NoRecordFoundException">when ver code not found</exception>
        /// <exception cref="InvalidOperationException">when resend count is maxed</exception>
        public void QueueAVerificationCodeResend(string token)
        {
            _handler.ResendVerificationCode(token);
        }


        /// <summary>
        /// Check if the stages match
        /// </summary>
        /// <param name="expectedStage"></param>
        /// <param name="actualStage"></param>
        /// <returns></returns>
        protected virtual bool IsStageCorrect(PSSUserRequestGenerationStage expectedStage, PSSUserRequestGenerationStage actualStage)
        {
            return expectedStage == actualStage;
        }


        /// <summary>
        /// Get the user details with the cbs User Id
        /// </summary>
        /// <param name="cbsUserId"></param>
        /// <returns>UserDetailsModel</returns>
        /// <exception cref="NoRecordFoundException"></exception>
        protected virtual UserDetailsModel GetUserDetails(long cbsUserId)
        {
            UserDetailsModel userDetails = _handler.GetUserDetailsForCBSUserId(cbsUserId);
            if(userDetails == null) { throw new NoRecordFoundException(string.Format("Could not find the cbs user for Id {0}", cbsUserId)); }
            return userDetails;
        }


        /// <summary>
        /// Get the details of the logged in user
        /// <para>Would return null if cbs user is not found</para>
        /// </summary>
        /// <returns>UserDetailsModel | null</returns>
        protected virtual UserDetailsModel GetLoggedInUserDetails(bool checkVerifiedGate = true)
        {
            try
            {
                IUser currentUser = _authenticationService.GetAuthenticatedUser();
                if (currentUser == null) { return null; }
                //check if the current user is a CBS user
                return GetUserDetails(currentUser.Id, checkVerifiedGate);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
            }
            return null;
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
            headerObj.TaxEntityCategorySettings = userDetails.CategoryVM.GetSettings();
            headerObj.IsAdministrator = userDetails.CBSUserVM.IsAdministrator;
            return headerObj;
        }



        /// <summary>
        /// Return the userderails and checks if the user has been verified or not
        /// Throws an exception if the user is logged in and has been verified
        /// </summary>
        /// <returns>UserDetailsModel if logged in or null</returns>
        /// <exception cref="UserNotAuthorizedForThisActionException"></exception>
        protected virtual UserDetailsModel CheckVerificationStatus()
        {
            UserDetailsModel userDetails = GetLoggedInUserDetails(false);
            if (userDetails != null && userDetails.CBSUserVM.Verified)
            {
                throw new UserNotAuthorizedForThisActionException();
            }
            return userDetails;
        }



        /// <summary>
        /// This method creates a new session for request processiong
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="serviceId"></param>
        /// <param name="serviceType"></param>
        /// <param name="servicePrefix"></param>
        /// <param name="serviceName"></param>
        /// <param name="subCategoryId"></param>
        /// <param name="subSubCategoryId"></param>
        /// <param name="stage"></param>
        /// <returns>PSSRequestStageModel</returns>
        protected virtual PSSRequestStageModel StartRequestSession(int categoryId, int serviceId, int serviceType, string servicePrefix, string serviceName, string serviceNote, int subCategoryId, int subSubCategoryId, PSSUserRequestGenerationStage stage, long cbsUserId = 0, bool isAdmin = false)
        {
            PSSRequestStageModel requestStage = new PSSRequestStageModel { CategoryId = categoryId, ServiceId = serviceId, ServiceType = serviceType, Stage = stage, CBSUserProfileId = cbsUserId, ServicePrefix = servicePrefix, ServiceName = serviceName, ServiceNote = serviceNote, SubCategoryId = subCategoryId, SubSubCategoryId = subSubCategoryId, IsAdministrator = isAdmin };
            Session.Add("PSSRequestStage", JsonConvert.SerializeObject(requestStage));
            return requestStage;
        }


        /// <summary>
        /// This method creates a new session for request processiong
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="serviceId"></param>
        /// <param name="serviceType"></param>
        /// <param name="servicePrefix"></param>
        /// <param name="serviceName"></param>
        /// <param name="subCategoryId"></param>
        /// <param name="subSubCategoryId"></param>
        /// <param name="stage"></param>
        /// <returns>PSSRequestStageModel</returns>
        protected virtual PSSRequestStageModel StartRequestSession(PSServiceVM serviceVM, int subCategoryId, int subSubCategoryId, PSSUserRequestGenerationStage stage, CBSUserVM cbsUserVM)
        {
            PSSRequestStageModel requestStage = new PSSRequestStageModel { CategoryId = serviceVM.CategoryId, ServiceId = serviceVM.ServiceId, ServiceType = serviceVM.ServiceType, Stage = stage, CBSUserProfileId = cbsUserVM.Id, ServicePrefix = serviceVM.ServicePrefix, ServiceName = serviceVM.ServiceName, ServiceNote = serviceVM.ServiceNote, SubCategoryId = subCategoryId, SubSubCategoryId = subSubCategoryId, IsAdministrator = cbsUserVM.IsAdministrator, HasDifferentialWorkFlow = serviceVM.HasDifferentialWorkFlow, TaxEntityId = cbsUserVM.TaxEntity.Id };

            Session.Add("PSSRequestStage", JsonConvert.SerializeObject(requestStage));
            return requestStage;
        }


        /// <summary>
        /// Redirect to verification gate
        /// </summary>
        /// <param name="cbsUserId"></param>
        protected virtual void CheckVerifiedGate(UserDetailsModel userDetails)
        {
            if (userDetails != null &&!userDetails.CBSUserVM.Verified)
            {
                throw new UserNotVerifiedException(string.Format("User has not been verified {0}", userDetails.CBSUserVM.Id));
            }
        }


        /// <summary>
        /// Get the details of the logged in user
        /// <para>Would return null if cbs user is not found</para>
        /// </summary>
        /// <returns>UserDetailsModel | null</returns>
        /// <exception cref="UserNotVerifiedException">If the checkverified gate is true, this exception will be thrown if the user need to be verified</exception>
        protected virtual UserDetailsModel GetUserDetails(int userPartId, bool checkVerifiedGate = true)
        {
            try
            {
                UserDetailsModel userDetails = _handler.GetUserDetails(userPartId);
                if (userDetails == null) { _authenticationService.SignOut(); return null; }
                if (checkVerifiedGate) { CheckVerifiedGate(userDetails); }
                return userDetails;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Checks that the session value for stage management is present
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <exception cref="Exception">Throw a generice exception is the session is not found</exception>
        protected virtual void CheckStageSessionValue(ref string errorMessage)
        {
            if (System.Web.HttpContext.Current.Session["PSSRequestStage"] == null)
            {
                Logger.Information(string.Format("User session has expired"));
                errorMessage = "Your session could not be continued. Please fill in your details and proceed to start new session.";
                throw new Exception(errorMessage);
            }
        }


        /// <summary>
        /// Get deserialized obj that has the stage model
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        protected virtual PSSRequestStageModel GetDeserializedSessionObj(ref string errorMessage)
        {
            try { return JsonConvert.DeserializeObject<PSSRequestStageModel>(System.Web.HttpContext.Current.Session["PSSRequestStage"].ToString()); }
            catch (Exception exception)
            {
                errorMessage = "Your session could not be continued. Please fill in your details and proceed to start new session.";
                Logger.Error(exception, string.Format("Exception getting the deserializing the session value {0} {1}", System.Web.HttpContext.Current.Session["PSSRequestStage"].ToString(), exception.Message));
                throw new Exception(errorMessage);
            }
        }

    }
}