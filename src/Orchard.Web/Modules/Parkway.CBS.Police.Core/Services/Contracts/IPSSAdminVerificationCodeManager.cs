using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.HelperModels;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IPSSAdminVerificationCodeManager<PSSAdminVerificationCode> : IDependency, IBaseManager<PSSAdminVerificationCode>
    {
        /// <summary>
        /// Gets verification code with specified id
        /// </summary>
        /// <param name="verificationCodeId"></param>
        /// <returns>PSSAdminVerificationCodeVM</returns>
        PSSAdminVerificationCodeVM GetVerificationCode(long verificationCodeId);

        /// <summary>
        /// Gets total resend count of code for specified verification type for pss admin user with specified id within from and to
        /// </summary>
        /// <param name="adminUserId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns>int</returns>
        int GetResendCountForTimePeriod(int adminUserId, CBS.Core.Models.Enums.VerificationType verificationType, System.DateTime from, System.DateTime to);

        /// <summary>
        /// Updates resend count for verification code with specified id
        /// </summary>
        /// <param name="verificationCodeId"></param>
        /// <param name="resendCount"></param>
        void UpdateVerificationCodeResendCount(long verificationCodeId, int resendCount);
    }
}
