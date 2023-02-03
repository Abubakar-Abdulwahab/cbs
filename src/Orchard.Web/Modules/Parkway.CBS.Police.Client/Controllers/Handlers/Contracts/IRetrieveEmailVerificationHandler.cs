using Orchard;
using Parkway.CBS.Core.HelperModels;
using System;

namespace Parkway.CBS.Police.Client.Controllers.Handlers.Contracts
{
    public interface IRetrieveEmailVerificationHandler : IDependency
    {
        /// <summary>
        /// Do token validation
        /// </summary>
        /// <param name="token"></param>
        /// <param name="userCodeInput"></param>
        /// <exception cref="TimeoutException">Indicates whether the token has expired or the token state has already been used</exception>
        /// <exception cref="NoRecordFoundException">Indicates when no verification item has been found</exception>
        /// <exception cref="Exception">Handle general exception</exception>
        ValidateVerReturnValue DoTokenValidation(string token, string userCodeInput);


        /// <summary>
        /// Send email address as sms to cbs user with specified id
        /// </summary>
        /// <param name="cbsUserId"></param>
        void SendEmailAddressAsSMSToUser(long cbsUserId);


        /// <summary>
        /// Check if token for email retrieval has expired
        /// </summary>
        /// <param name="token"></param>
        /// <returns>bool</returns>
        /// <exception cref="Exception"></exception>
        /// <exception cref="UserNotAuthorizedForThisActionException">If the cbs user has already been verified and this is an account verification request, this exception will be thrown</exception>
        bool TokenHasExpired(string token);


        /// <summary>
        /// Resend verification token
        /// </summary>
        /// <param name="token"></param>
        /// <exception cref="NoRecordFoundException">when ver code not found</exception>
        /// <exception cref="InvalidOperationException">when resend count is maxed</exception>
        void ResendVerificationCode(string token);
    }
}
