using Orchard;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface IVerificationCodeItemsManager<VerificationCodeItems> : IDependency, IBaseManager<VerificationCodeItems>
    {
        /// <summary>
        /// Update verification code item state
        /// </summary>
        /// <param name="verificationCodeId"></param>
        /// <param name="verificationState"></param>
        void UpdateVerificationItemState(long verificationCodeId, Models.Enums.VerificationState verificationState);
    }
}
