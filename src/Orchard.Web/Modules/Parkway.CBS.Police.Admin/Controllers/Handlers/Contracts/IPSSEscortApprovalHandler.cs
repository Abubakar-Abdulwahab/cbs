using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts
{
    public interface IPSSEscortApprovalHandler : IDependency
    {
        /// <summary>
        /// Gets police officer with specified service number from HR system
        /// </summary>
        /// <param name="serviceNumber"></param>
        /// <returns></returns>
        APIResponse GetPoliceOfficer(string serviceNumber);

        /// <summary>
        /// Gets formations selected to nominate officers by the AIG of the squad with the specified escort squad allocation id
        /// </summary>
        /// <param name="escortSquadAllocationId"></param>
        /// <param name="escortSquadAllocationGroup"></param>
        /// <returns></returns>
        IEnumerable<AIGFormationVM> GetFormationsAllocatedToSquad(long escortSquadAllocationId, long escortSquadAllocationGroup);

        /// <summary>
        /// Gets proposed escort officers for a request with the specified request id allocated from the command with the specified command id
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="commandId"></param>
        /// <returns></returns>
        APIResponse GetProposedEscortOfficers(long requestId, int commandId);

        /// <summary>
        /// Gets number of officers requested from formation with the specified formation allocation id
        /// </summary>
        /// <param name="formationAllocationId"></param>
        /// <param name="allocationGroupId"></param>
        /// <returns></returns>
        APIResponse GetNumberOfOfficersRequestedFromFormation(long formationAllocationId, long allocationGroupId);

        /// <summary>
        /// Performs user specific approval logic
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="userId"></param>
        EscortApprovalMessage ProcessCustomApproval(long requestId, int userId);
    }
}
