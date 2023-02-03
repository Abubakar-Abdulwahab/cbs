using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.HelperModels;

namespace Parkway.CBS.Police.API.Controllers.Handlers.Contracts
{
    public interface IPSSProxyAuthenticationHandler : IDependency
    {
        /// <summary>
        /// Validates the user
        /// </summary>
        /// <param name="authenticationModel"></param>
        /// <returns></returns>
        APIResponse ValidateUserSigninCredentials(PSSProxyAuthenticationModel authenticationModel);

        /// <summary>
        /// Validates the username belongs to a valid user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        APIResponse ValidateUserName(PSSProxyAuthenticationModel model);
    }
}
