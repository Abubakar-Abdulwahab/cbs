using Orchard;
using Parkway.CBS.Police.Core.HelperModels;

namespace Parkway.CBS.Police.API.Controllers.Handlers.Contracts
{
    public interface IUSSDRequestHandler : IDependency
    {
        /// <summary>
        /// Process ussd approval request
        /// </summary>
        /// <param name="model"></param>
        /// <returns>USSDAPIResponse</returns>
        USSDAPIResponse ProcessApprovalRequest(USSDRequestModel model);
    }
}
