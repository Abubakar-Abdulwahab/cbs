using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.VM;
using System;

namespace Parkway.CBS.Police.Core.CoreServices.Contracts
{
    public interface ICorePSSAdminVerificationService : IDependency
    {

        /// <summary>
        /// Queue up a verification token to be sent to the user
        /// for specified verification type
        /// </summary>
        /// <param name="user"></param>
        /// <param name="verificationModel">AccountVerificationEmailNotificationModel</param>
        /// <param name="verificationType">Enums.VerificationType</param>
        /// <exception cref="CouldNotSaveRecord"></exception>
        VerTokenResult QueueVerificationToken(PSSAdminUsersVM user, AccountVerificationEmailNotificationModel verificationModel, VerificationType verificationType);


        /// <summary>
        /// Resends code for specified verification type provided the max number of resend count has not yet been exceed within the period defined by the from and to parameters
        /// </summary>
        /// <param name="token"></param>
        /// <param name="verificationModel"></param>
        /// <param name="verificationType"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="resendLimit"></param>
        /// <exception cref="NoRecordFoundException">throws this when no record is found</exception>
        /// <exception cref="InvalidOperationException">throws this when the number of resends has exceeded limit</exception>
        void ResendCodeNotification(string token, AccountVerificationEmailNotificationModel verificationModel, VerificationType verificationType, DateTime from, DateTime to, int resendLimit);


        /// <summary>
        /// Validate token against user input for specified verification type
        /// </summary>
        /// <param name="token"></param>
        /// <param name="userCodeInput"></param>
        /// <param name="verificationType"></param>
        /// <exception cref="TimeoutException">Indicates whether the token has expired or the token state has already been used</exception>
        /// <exception cref="NoRecordFoundException">Indicates when no verification item has been found</exception>
        /// <exception cref="Exception">Handle general exception</exception>
        PSSAdminValidateVerReturnValue ValidateVerificationCode(string token, string userCodeInput, VerificationType verificationType);


        /// <summary>
        /// Check if token has expired
        /// </summary>
        /// <param name="token"></param>
        /// <returns>bool</returns>
        bool CheckForTokenExpiry(string token);
    }
}
