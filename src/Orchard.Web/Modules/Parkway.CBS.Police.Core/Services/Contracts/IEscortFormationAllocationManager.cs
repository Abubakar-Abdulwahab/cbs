using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IEscortFormationAllocationManager<EscortFormationAllocation> : IDependency, IBaseManager<EscortFormationAllocation>
    {
        /// <summary>
        /// Gets formations selected to nominate officers by the AIG of the squad with the specified escort squad allocation id
        /// </summary>
        /// <param name="escortSquadAllocationId"></param>
        /// <param name="escortSquadAllocationGroupId"></param>
        /// <returns></returns>
        IEnumerable<AIGFormationVM> GetFormationsAllocatedToSquad(long escortSquadAllocationId, long escortSquadAllocationGroupId);

        /// <summary>
        /// Gets number of officers requested from the formation with the specified formation allocation id
        /// </summary>
        /// <param name="formationAllocationId"></param>
        /// <param name="allocationGroupId"></param>
        /// <returns></returns>
        int GetNumberOfOfficersRequestedFromFormation(long formationAllocationId, long allocationGroupId);
    }
}
