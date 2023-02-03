using Orchard;
using Parkway.CBS.Core.Services.Contracts;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface ISecretariatRoutingLevelManager<SecretariatRoutingLevel> : IDependency, IBaseManager<SecretariatRoutingLevel>
    {
        /// <summary>
        /// Save secretariat routing level for request with specified id at selected request stage for admin user
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="selectedRequestStage"></param>
        /// <param name="adminUserId"></param>
        /// <param name="modelName"></param>
        /// <returns>bool</returns>
        bool SaveSecretariatRoutingLevel(long requestId, int selectedRequestStage, int adminUserId, string modelName);
    }
}
