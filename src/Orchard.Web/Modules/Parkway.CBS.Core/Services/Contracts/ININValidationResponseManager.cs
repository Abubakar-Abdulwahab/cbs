using Orchard;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface ININValidationResponseManager<NINValidationResponse> : IDependency, IBaseManager<NINValidationResponse>
    {
        /// <summary>
        /// Gets the last nin validation response with specified nin
        /// </summary>
        /// <param name="nin"></param>
        /// <returns></returns>
        LatestNINValidationResponseVM GetNINValidationResponse(string nin);
        /// <summary>
        /// Updates tax entity info with most recent validation response from NIN
        /// </summary>
        /// <param name="nin"></param>
        void UpdateTaxEntityInfoWithValidationResponseForNIN(string nin);

        /// <summary>
        /// Updates CBS User Info with most recent validation response from NIN
        /// </summary>
        /// <param name="nin"></param>
        void UpdateCBSUserInfoWithValidationResponseForNIN(string nin);
    }
}
