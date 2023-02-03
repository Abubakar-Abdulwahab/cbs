using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.VM
{
    public class SquadronLeaderApprovalVM
    {
        public IEnumerable<ProposedEscortOffficerVM> ProposedEscortOffficers { get; set; }

        public int NumberOfOfficersRequested { get; set; }

        public long FormationAllocationId { get; set; }

        public long AllocationGroupId { get; set; }
    }
}