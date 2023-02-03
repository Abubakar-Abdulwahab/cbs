using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.VM
{
    public class AIGApprovalVM
    {
        public IEnumerable<CBS.Core.Models.StateModel> States { get; set; }

        public IEnumerable<DTO.EscortFormationAllocationDTO> FormationsAllocated { get; set;}

        public int NumberOfOfficersRequested { get; set; }

        public long RequestId { get; set; }

        public CanApproveEscortVM CanApprove { get; set; }
    }
}