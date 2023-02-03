using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;

namespace Parkway.CBS.Core.Mail.Provider.Contracts
{
    public interface IEmailProvider : IDependency
    {
        EmailProvider GetEmailNotificationProvider { get; }

        /// <summary>
        /// Send verification code for a user verification
        /// </summary>
        /// <param name="verificationCodeObj"></param>
        /// <param name="verificationModel"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        bool AccountVerification(VerificationCode verificationCodeObj, AccountVerificationEmailNotificationModel verificationModel, string code);


        /// <summary>
        /// Sends the email defined in the <paramref name="emailModel"/> and logs the email sent depending on the value of <paramref name="logNotification"/>
        /// </summary>
        /// <param name="emailModel"></param>
        /// <param name="logNotification"></param>
        /// <returns></returns>
        bool SendEmail(EmailNotificationModel emailModel, bool logNotification = false);
    }
}
