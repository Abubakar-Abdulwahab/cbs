using Orchard;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models.Enums;

namespace Parkway.CBS.Police.Core.PSSServiceType.Contracts
{
    public interface IPSSServiceTypeUSSDApprovalImpl : IDependency
    {
        PSSServiceTypeDefinition GetServiceTypeDefinition { get; }

        /// <summary>
        /// Process USSD approval request
        /// </summary>
        /// <param name="model"></param>
        /// <returns>USSDAPIResponse</returns>
        USSDAPIResponse ProcessRequest(USSDRequestModel model);
    }
}
