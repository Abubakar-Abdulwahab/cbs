using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models.Enums;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface IVerificationCodeManager<VerificationCode> : IDependency, IBaseManager<VerificationCode>
    {

        /// <summary>
        /// Get model for token regeneration
        /// </summary>
        /// <param name="verId"></param>
        /// <returns>ModelForTokenRegeneration</returns>
        ModelForTokenRegeneration GetCBSUserDetailsWithVerificationId(long verId);

        /// <summary>
        /// Gets verification code with specified id
        /// </summary>
        /// <param name="verificationCodeId"></param>
        /// <returns>VerificationCodeVM</returns>
        VerificationCodeVM GetVerificationCode(long verificationCodeId);

        /// <summary>
        /// Gets verification code with specified id
        /// </summary>
        /// <param name="verificationCodeId"></param>
        /// <param name="verificationType"></param>
        /// <returns>VerificationCodeVM</returns>
        VerificationCodeVM GetVerificationCode(long verificationCodeId, VerificationType verificationType);

        /// <summary>
        /// Gets total resend count of code for specified verification type for cbs user with specified id within from and to
        /// </summary>
        /// <param name="cbsUserId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        int GetResendCountForTimePeriod(long cbsUserId, Models.Enums.VerificationType verificationType, System.DateTime from, System.DateTime to);

        /// <summary>
        /// Updates resend count for verification code with specified id
        /// </summary>
        /// <param name="verificationCodeId"></param>
        /// <param name="resendCount"></param>
        void UpdateVerificationCodeResendCount(long verificationCodeId, int resendCount);
    }
}
