using Orchard.Security;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.API.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using System.Net;

namespace Parkway.CBS.Police.API.Controllers.Handlers
{
    public class PSSProxyAuthenticationHandler : IPSSProxyAuthenticationHandler
    {
        private readonly IMembershipService _membershipService;

        public PSSProxyAuthenticationHandler(IMembershipService membershipService)
        {
            _membershipService = membershipService;
        }

        /// <summary>
        /// Validates the user
        /// </summary>
        /// <param name="authenticationModel"></param>
        /// <returns></returns>
        public APIResponse ValidateUserSigninCredentials(PSSProxyAuthenticationModel authenticationModel)
        {
            IUser ouser = _membershipService.ValidateUser(authenticationModel.UserName, authenticationModel.Password);

            if (ouser == null)
            {
                return new APIResponse { Error = true, ResponseObject = "Invalid login credentials", StatusCode = HttpStatusCode.OK };
            }

            return new APIResponse { ResponseObject = ouser.UserName, StatusCode = HttpStatusCode.OK };
        }

        /// <summary>
        /// Validates the username belongs to a valid user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public APIResponse ValidateUserName(PSSProxyAuthenticationModel model)
        {
            if (_membershipService.GetUser(model.UserName.Trim()) != null)
            {
                return new APIResponse { ResponseObject = true, StatusCode = HttpStatusCode.OK };
            }

            return new APIResponse{ Error = true, ResponseObject = "Invalid username", StatusCode = HttpStatusCode.OK };
        }
    }
}