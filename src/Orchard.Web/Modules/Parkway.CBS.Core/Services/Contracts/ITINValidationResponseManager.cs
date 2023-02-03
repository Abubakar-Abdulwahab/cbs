using Orchard;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface ITINValidationResponseManager<TINValidationResponse> : IDependency, IBaseManager<TINValidationResponse>
    {
        /// <summary>
        /// Updates tax entity info with most recent validation response from TIN
        /// </summary>
        /// <param name="tin"></param>
        void UpdateTaxEntityInfoWithValidationResponseForTIN(string tin);

        /// <summary>
        /// Updates CBS User Info with most recent validation response from TIN
        /// </summary>
        /// <param name="tin"></param>
        void UpdateCBSUserInfoWithValidationResponseForTIN(string tin);
    }
}
