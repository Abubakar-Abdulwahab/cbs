using Orchard;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Police.Client.Controllers.Handlers.Contracts
{
    public interface IAccountVerificationHandler : IDependency
    {

        /// <summary>
        /// Do token validation
        /// </summary>
        /// <param name="token"></param>
        /// <param name="userCodeInput"></param>
        /// <exception cref="TimeoutException">Indicates whether the token has expired or the token state has already been used</exception>
        /// <exception cref="NoRecordFoundException">Indicates when no verification item has been found</exception>
        /// <exception cref="Exception">Handle general exception</exception>
        /// <returns>ValidateVerReturnValue</returns>
        ValidateVerReturnValue DoTokenValidation(string token, string userCodeInput, bool isPasswordReset = false);


        /// <summary>
        /// Check if token has expired and if this is a reset password request
        /// </summary>
        /// <param name="token"></param>
        /// <returns>bool</returns>
        bool TokenHasExpired(string token, bool isPasswordReset = false);


        /// <summary>
        /// Get a the CBSUser Id for this token
        /// </summary>
        /// <param name="token"></param>
        /// <returns>long</returns>
        long GetUserIdAttachedToToken(string token);


        /// <summary>
        /// Get a the CBSUser Id for this token and also the redirect response instructuions
        /// </summary>
        /// <param name="token"></param>
        /// <returns>TokenExpiryResponse</returns>
        TokenExpiryResponse GetTokenExpiryResponseToToken(string token);

    }
}
