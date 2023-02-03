using Orchard;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface IBVNValidationResponseManager<BVNValidationResponse> : IDependency, IBaseManager<BVNValidationResponse>
    {
        /// <summary>
        /// Updates tax entity info with most recent validation response from BVN
        /// </summary>
        /// <param name="bvn"></param>
        void UpdateTaxEntityInfoWithValidationResponseForBVN(string bvn);

        /// <summary>
        /// Updates CBS User Info with most recent validation response from BVN
        /// </summary>
        /// <param name="bvn"></param>
        void UpdateCBSUserInfoWithValidationResponseForBVN(string bvn);
    }
}
