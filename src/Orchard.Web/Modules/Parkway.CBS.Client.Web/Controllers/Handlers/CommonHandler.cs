using Orchard.Logging;
using Orchard.Security;
using Parkway.CBS.Client.Web.Controllers.Handlers.Contracts;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Client.Web.Controllers.Handlers
{
    public class CommonHandler: ICommonHandler
    {
        public ILogger Logger { get; set; }
        private readonly IAuthenticationService _authenticationService;
        private readonly ICBSUserManager<CBSUser> _cbsUserService;

        public CommonHandler(IAuthenticationService authenticationService, ICBSUserManager<CBSUser> cbsUserService)
        {
            Logger = NullLogger.Instance;
            _authenticationService = authenticationService;
            _cbsUserService = cbsUserService;
        }

        /// <summary>
        /// Get the details of the logged in user
        /// <para>Would return an initialized object at all time, so check if entity is null to validate if there is a user for this request</para>
        /// </summary>
        /// <returns>UserDetailsModel</returns>
        public UserDetailsModel GetLoggedInUserDetails()
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
                    //_authenticationService.SignOut();
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
        /// Get header object
        /// </summary>
        /// <returns></returns>
        public HeaderObj GetHeaderObj()
        {
            var userDetails = GetLoggedInUserDetails();
            HeaderObj obj = new HeaderObj { ShowSignin = true };
            if (userDetails != null && userDetails.Entity != null) { obj.IsLoggedIn = true; obj.DisplayText = userDetails.Name; obj.CategoryId = userDetails.Category.Id; }
            else { obj.IsLoggedIn = false; }
            return obj;
        }

        /// <summary>
        /// Fill header obj
        /// <para>will always return an instance of the return object</para>
        /// </summary>
        /// <param name="userDetails"></param>
        /// <returns>HeaderObj</returns>
        public HeaderObj HeaderFiller(UserDetailsModel userDetails)
        {
            HeaderObj headerObj = new HeaderObj { };
            if (userDetails == null) return headerObj;
            headerObj.IsLoggedIn = true;
            headerObj.ShowSignin = true;
            headerObj.DisplayText = userDetails.Name;
            headerObj.CategoryId = userDetails.Category.Id;
            return headerObj;
        }

    }
}