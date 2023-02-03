using System;
using Orchard;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Core.Services.Contracts;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IEscortSquadAllocationGroupManager<EscortSquadAllocationGroup> : IDependency, IBaseManager<EscortSquadAllocationGroup>
    {

        /// <summary>
        /// Get the current process stage this application is on
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns>EscortSquadAllocationGroupDTO</returns>
        EscortSquadAllocationGroupDTO GetProcessStage(Int64 requestId);
    }
}
