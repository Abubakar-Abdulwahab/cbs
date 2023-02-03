using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.DTO;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IEscortProcessFlowManager<EscortProcessFlow> : IDependency, IBaseManager<EscortProcessFlow>
    {

        /// <summary>
        /// Get the process flow object for this admin user
        /// </summary>
        /// <param name="adminUserId"></param>
        /// <param name="commandTypeId"></param>
        /// <returns>List{EscortProcessFlowDTO}</returns>
        List<EscortProcessFlowDTO> GetProcessFlowObject(int adminUserId, int commandTypeId);

        /// <summary>
        /// Gets role of admin attached to escort process flow with the specified escort process stage definition id
        /// </summary>
        /// <param name="processStageDefinitionId"></param>
        /// <param name="commandTypeId"></param>
        /// <returns></returns>
        int GetRoleFromAdminInProcessFlowWithProcessStageDefinition(int processStageDefinitionId, int commandTypeId);
    }
}

