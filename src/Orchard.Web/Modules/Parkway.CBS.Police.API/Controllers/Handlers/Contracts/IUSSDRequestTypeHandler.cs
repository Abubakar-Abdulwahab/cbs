using Orchard;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models.Enums;

namespace Parkway.CBS.Police.API.Controllers.Handlers.Contracts
{
    public interface IUSSDRequestTypeHandler : IDependency
    {
        USSDRequestType GetRequestType { get; }

        /// <summary>
        /// Process ussd request
        /// </summary>
        /// <param name="model"></param>
        /// <returns>USSDAPIResponse</returns>
        USSDAPIResponse StartRequest(USSDRequestModel model);
    }
}
