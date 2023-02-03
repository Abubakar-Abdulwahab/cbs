using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Police.Core.HelperModels;
using System;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts
{
    public interface IAccountWalletPaymentApprovalAJAXHandler : IDependency
    {
        /// <summary>
        /// Get the verification token
        /// </summary>
        /// <param name="userPartRecordId"></param>
        /// <param name="verificationType">Enums.VerificationType</param>
        /// <param name="redirectObj">if a redirect is needed after verification provide this object</param>
        /// <returns>string</returns>
        string ProvideVerificationToken(int userPartRecordId, VerificationType verificationType, RedirectReturnObject redirectObj = null);


        /// <summary>
        /// Do token validation
        /// </summary>
        /// <param name="token"></param>
        /// <param name="userCodeInput"></param>
        /// <exception cref="TimeoutException">Indicates whether the token has expired or the token state has already been used</exception>
        /// <exception cref="NoRecordFoundException">Indicates when no verification item has been found</exception>
        /// <exception cref="Exception">Handle general exception</exception>
        PSSAdminValidateVerReturnValue DoTokenValidation(string token, string userCodeInput);


        /// <summary>
        /// Resend verification token
        /// </summary>
        /// <param name="token"></param>
        /// <exception cref="NoRecordFoundException">when ver code not found</exception>
        /// <exception cref="InvalidOperationException">when resend count is maxed</exception>
        void ResendVerificationCode(string token);


        /// <summary>
        /// Check if token for account wallet payment verification has expired
        /// </summary>
        /// <param name="token"></param>
        /// <returns>bool</returns>
        /// <exception cref="Exception"></exception>
        bool TokenHasExpired(string token);
    }
}
