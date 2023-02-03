using Orchard.Logging;
using Orchard.Security;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Module.Web.Controllers.Handlers.Contracts;
using System;

namespace Parkway.CBS.Module.Web.Controllers.Handlers
{
    public class HandlerComposition : IHandlerComposition
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ICBSUserManager<CBSUser> _cbsUserService;
        public ILogger Logger { get; set; }


        public HandlerComposition(IAuthenticationService authenticationService, ICBSUserManager<CBSUser> cbsUserService)
        {
            _authenticationService = authenticationService;
            _cbsUserService = cbsUserService;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// Get the details of the logged in user
        /// <para>Would return null if cbs user is not found</para>
        /// </summary>
        /// <returns>UserDetailsModel | null</returns>
        /// <exception cref="UserNotVerifiedException">If the checkverified gate is true, this exception will be thrown if the user need to be verified</exception>
        protected virtual UserDetailsModel GetUserDetails(int userPartId, bool checkVerifiedGate = false)
        {
            try
            {
                UserDetailsModel userDetails =  _cbsUserService.GetUserDetails(userPartId);
                if (userDetails == null) { _authenticationService.SignOut(); return null; }
                if (checkVerifiedGate) { throw new NotImplementedException(); }
                return userDetails;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Get the details of the logged in user
        /// <para>Would return null if cbs user is not found</para>
        /// </summary>
        /// <returns>UserDetailsModel | null</returns>
        public virtual UserDetailsModel GetLoggedInUserDetails(bool checkVerifiedGate = false)
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

    }
}