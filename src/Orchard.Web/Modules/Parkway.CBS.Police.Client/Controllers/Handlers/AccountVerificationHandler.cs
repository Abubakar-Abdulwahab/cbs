using System;
using Newtonsoft.Json;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;

namespace Parkway.CBS.Police.Client.Controllers.Handlers
{
    public class AccountVerificationHandler : IAccountVerificationHandler
    {
        private readonly ICoreCBSUserVerification _coreUserVer;

        public AccountVerificationHandler(ICoreCBSUserVerification coreUserVer)
        {
            _coreUserVer = coreUserVer;
        }


        /// <summary>
        /// Do token validation
        /// </summary>
        /// <param name="token"></param>
        /// <param name="userCodeInput"></param>
        /// <exception cref="TimeoutException">Indicates whether the token has expired or the token state has already been used</exception>
        /// <exception cref="NoRecordFoundException">Indicates when no verification item has been found</exception>
        /// <exception cref="Exception">Handle general exception</exception>
        public ValidateVerReturnValue DoTokenValidation(string token, string userCodeInput, bool isPasswordReset = false)
        {
            return _coreUserVer.ValidateVerificationCode(token, userCodeInput, isPasswordReset);
        }


        /// <summary>
        /// Check if token has expired and if this is a reset password request
        /// </summary>
        /// <param name="token"></param>
        /// <returns>bool</returns>
        /// <exception cref="Exception"></exception>
        /// <exception cref="UserNotAuthorizedForThisActionException">If the cbs user has already been verified, this exception will be thrown</exception>
        public bool TokenHasExpired(string token, bool isPasswordReset = false)
        {
            return _coreUserVer.CheckForTokenExpiry(token, isPasswordReset);
        }


        /// <summary>
        /// Get a the CBSUser Id for this token
        /// </summary>
        /// <param name="token"></param>
        /// <returns>string</returns>
        public long GetUserIdAttachedToToken(string token)
        {
            string decpToken = Util.LetsDecrypt(token);
            VerTokenEncryptionObject enobj = JsonConvert.DeserializeObject<VerTokenEncryptionObject>(decpToken);
            return _coreUserVer.GetCBSUserId(enobj.VerId);
        }


        /// <summary>
        /// Get a the CBSUser Id for this token and also the redirect response instructuions
        /// </summary>
        /// <param name="token"></param>
        /// <returns>TokenExpiryResponse</returns>
        public TokenExpiryResponse GetTokenExpiryResponseToToken(string token)
        {
            string decpToken = Util.LetsDecrypt(token);
            VerTokenEncryptionObject enobj = JsonConvert.DeserializeObject<VerTokenEncryptionObject>(decpToken);
            ModelForTokenRegeneration queryResponse = _coreUserVer.GetCBSUserDetailsByVerificationCodeId(enobj.VerId);
            return new TokenExpiryResponse { RedirectObj = enobj.RedirectObj, CBSUserId = queryResponse.CBSUserVM.Id, CBSUser = queryResponse.CBSUserVM, TaxEntity = queryResponse.TaxPayerProfileVM };
        }


    }
}