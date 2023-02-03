using Orchard.Logging;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Client.Controllers.Handlers
{
    public class RetrieveEmailVerificationHandler : IRetrieveEmailVerificationHandler
    {
        private readonly ICoreCBSUserVerification _coreUserVer;
        ILogger Logger { get; set; }
        public RetrieveEmailVerificationHandler(ICoreCBSUserVerification coreUserVer)
        {
            _coreUserVer = coreUserVer;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// Do token validation
        /// </summary>
        /// <param name="token"></param>
        /// <param name="userCodeInput"></param>
        /// <exception cref="TimeoutException">Indicates whether the token has expired or the token state has already been used</exception>
        /// <exception cref="NoRecordFoundException">Indicates when no verification item has been found</exception>
        /// <exception cref="Exception">Handle general exception</exception>
        public ValidateVerReturnValue DoTokenValidation(string token, string userCodeInput)
        {
            return _coreUserVer.ValidateVerificationCode(token, userCodeInput, CBS.Core.Models.Enums.VerificationType.RetrieveEmail);
        }


        /// <summary>
        /// Send email address as sms to cbs user with specified id
        /// </summary>
        /// <param name="cbsUserId"></param>
        public void SendEmailAddressAsSMSToUser(long cbsUserId)
        {
            _coreUserVer.SendEmailAddressSMSToCBSUser(cbsUserId);
        }


        /// <summary>
        /// Check if token for email retrieval has expired
        /// </summary>
        /// <param name="token"></param>
        /// <returns>bool</returns>
        /// <exception cref="Exception"></exception>
        /// <exception cref="UserNotAuthorizedForThisActionException">If the cbs user has already been verified and this is an account verification request, this exception will be thrown</exception>
        public bool TokenHasExpired(string token)
        {
            return _coreUserVer.CheckForTokenExpiry(token, CBS.Core.Models.Enums.VerificationType.RetrieveEmail);
        }


        /// <summary>
        /// Resend verification token
        /// </summary>
        /// <param name="token"></param>
        /// <exception cref="NoRecordFoundException">when ver code not found</exception>
        /// <exception cref="InvalidOperationException">when resend count is maxed</exception>
        public void ResendVerificationCode(string token)
        {
            try
            {
                int resendLimit = 5;
                string sresendLimit = AppSettingsConfigurations.GetSettingsValue(CBS.Core.Models.Enums.AppSettingEnum.RetrieveEmailVerificationResendCodeLimit);
                if (!string.IsNullOrEmpty(sresendLimit)) { int.TryParse(sresendLimit, out resendLimit); }
                _coreUserVer.ResendCodeNotification(token, CBS.Core.Models.Enums.VerificationType.RetrieveEmail, DateTime.Now.Date, DateTime.Now.Date.AddDays(1).AddMilliseconds(-1), resendLimit);
            }
            catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }
    }
}