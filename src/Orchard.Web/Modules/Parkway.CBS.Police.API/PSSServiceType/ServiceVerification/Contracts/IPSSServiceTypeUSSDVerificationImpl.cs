using Orchard;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models.Enums;

namespace Parkway.CBS.Police.API.PSSServiceType.ServiceVerification.Contracts
{
    public interface IPSSServiceTypeUSSDVerificationImpl : IDependency
    {
        PSSServiceTypeDefinition GetServiceTypeDefinition { get; }

        /// <summary>
        /// Process USSD service verification request
        /// </summary>
        /// <param name="model"></param>
        /// <returns>USSDAPIResponse</returns>
        USSDAPIResponse ProcessRequest(USSDRequestModel model);
    }
}
