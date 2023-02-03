using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Core.HTTP.Handlers.Contracts
{
    public interface ICoreCBSUserVerification : IDependency
    {

        /// <summary>
        /// Queue up a verification token to be sent to the user
        /// for account verification
        /// </summary>
        /// <param name="cbsUserId">long</param>
        /// <param name="verificationModel"></param>
        /// <param name="verificationType">Enums.VerificationType</param>
        /// <exception cref="CouldNotSaveRecord"></exception>
        VerTokenResult QueueVerificationToken(long cbsUserId, AccountVerificationEmailNotificationModel verificationModel, Models.Enums.VerificationType verificationType);

        /// <summary>
        /// Queue up a verification token to be sent to the user
        /// for account verification
        /// </summary>
        /// <param name="cbsUser"></param>
        /// <param name="verificationModel">AccountVerificationEmailNotificationModel</param>
        /// <param name="verificationType">Enums.VerificationType</param>
        /// <exception cref="CouldNotSaveRecord"></exception>
        VerTokenResult QueueVerificationToken(CBSUserVM cbsUser, AccountVerificationEmailNotificationModel verificationModel, Models.Enums.VerificationType verificationType);

        /// <summary>
        /// Queue a new verification code
        /// </summary>
        /// <param name="code"></param>
        /// <param name="verificationModel"></param>
        VerTokenResult QueueANewResendToken(VerificationCode code, AccountVerificationEmailNotificationModel verificationModel);

        /// <summary>
        /// Queue up a verification token to be sent to the user
        /// for specified verification type (SMS only)
        /// </summary>
        /// <param name="cbsUser"></param>
        /// <param name="verificationType">Enums.VerificationType</param>
        /// <exception cref="CouldNotSaveRecord"></exception>
        VerTokenResult QueueVerificationToken(CBSUserVM cbsUser, Models.Enums.VerificationType verificationType);

        /// <summary>
        /// Resend a new verification code (SMS only)
        /// </summary>
        /// <param name="verCodeObj"></param>
        /// <exception cref="CouldNotSaveRecord"></exception>
        VerTokenResult QueueANewResendToken(VerificationCodeVM verCodeObj);

        /// <summary>
        /// Validate token against user input
        /// </summary>
        /// <param name="token"></param>
        /// <param name="userCodeInput"></param>
        /// <exception cref="TimeoutException">Indicates whether the token has expired or the token state has already been used</exception>
        /// <exception cref="NoRecordFoundException">Indicates when no verification item has been found</exception>
        /// <exception cref="Exception">Handle general exception</exception>
        /// <returns>ValidateVerReturnValue</returns>
        ValidateVerReturnValue ValidateVerificationCode(string token, string userCodeInput, bool isPasswordReset = false);

        /// <summary>
        /// Sends email address as sms to specified user
        /// </summary>
        /// <param name="cbsUserId"></param>
        void SendEmailAddressSMSToCBSUser(long cbsUserId);

        /// <summary>
        /// Validate token against user input for specified verification type
        /// </summary>
        /// <param name="token"></param>
        /// <param name="userCodeInput"></param>
        /// <param name="verificationType"></param>
        /// <exception cref="TimeoutException">Indicates whether the token has expired or the token state has already been used</exception>
        /// <exception cref="NoRecordFoundException">Indicates when no verification item has been found</exception>
        /// <exception cref="Exception">Handle general exception</exception>
        ValidateVerReturnValue ValidateVerificationCode(string token, string userCodeInput, Models.Enums.VerificationType verificationType);


        /// <summary>
        /// Get CBS user attached to this verification code
        /// </summary>
        /// <param name="verId"></param>
        /// <returns></returns>
        long GetCBSUserId(long verId);        


        /// <summary>
        /// Get the verification code db object with this Id
        /// </summary>
        /// <param name="verId"></param>
        /// <returns>VerificationCode</returns>
        VerificationCode GetVerificationObject(long verId);


        /// <summary>
        /// Resend code
        /// </summary>
        /// <param name="token"></param>
        void ResendCodeNotification(string token, AccountVerificationEmailNotificationModel verificationModel);


        /// <summary>
        /// Resends code for specified verification type provided the max number of resend count has not yet been exceed within the period defined by the from and to parameters
        /// </summary>
        /// <param name="token"></param>
        /// <param name="verificationType"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="resendLimit"></param>
        /// <exception cref="NoRecordFoundException">throws this when no record is found</exception>
        /// <exception cref="InvalidOperationException">throws this when the number of resends has exceeded limit</exception>
        void ResendCodeNotification(string token, Models.Enums.VerificationType verificationType, System.DateTime from, System.DateTime to, int resendLimit);


        /// <summary>
        /// Check if token has expired and if this is a reset password request
        /// </summary>
        /// <param name="token"></param>
        /// <returns>bool</returns>
        /// <exception cref="UserNotAuthorizedForThisActionException">If the cbs user has already been verified, this exception will be thrown</exception>
        bool CheckForTokenExpiry(string token, bool isPasswordReset = false);


        /// <summary>
        /// Check if token has expired
        /// </summary>
        /// <param name="token"></param>
        /// <param name="verificationType"></param>
        /// <returns>bool</returns>
        /// <exception cref="UserNotAuthorizedForThisActionException">If the cbs user has already been verified and this is an account verification request, this exception will be thrown</exception>
        bool CheckForTokenExpiry(string token, Models.Enums.VerificationType verificationType);


        /// <summary>
        /// Get model user details model for this verification Id
        /// </summary>
        /// <param name="verId"></param>
        /// <returns>ModelForTokenRegeneration</returns>
        ModelForTokenRegeneration GetCBSUserDetailsByVerificationCodeId(long verId);

    }
}
