using Orchard;
using Parkway.CBS.Core.Services.Contracts;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IPSSEscortSettingsManager<PSSEscortSettings> : IDependency, IBaseManager<PSSEscortSettings>
    {

        /// <summary>
        /// Check if admin can assign officers
        /// </summary>
        /// <param name="adminUserId"></param>
        /// <param name="flowDefinitionId"></param>
        /// <returns>bool</returns>
        bool CanAdminAssignOfficers(int adminUserId, int flowDefinitionId);


        /// <summary>
        /// Get the settings Id for this flow definition
        /// </summary>
        /// <param name="workFlowdefinitionId"></param>
        /// <returns>int</returns>
        int GetEscortSettingsId(int workFlowdefinitionId);

    }
}
