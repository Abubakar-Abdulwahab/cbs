using Orchard;
using Parkway.CBS.Core.Services.Contracts;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IPSSAdminVerificationCodeItemsManager<PSSAdminVerificationCodeItems> : IDependency, IBaseManager<PSSAdminVerificationCodeItems>
    {
        /// <summary>
        /// Update verification code item state
        /// </summary>
        /// <param name="verificationCodeId"></param>
        /// <param name="verificationState"></param>
        void UpdateVerificationItemState(long verificationCodeId, CBS.Core.Models.Enums.VerificationState verificationState);
    }
}
