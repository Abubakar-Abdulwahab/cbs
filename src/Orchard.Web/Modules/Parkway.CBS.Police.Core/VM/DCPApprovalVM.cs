using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.VM
{
    public class DCPApprovalVM
    {
        public IEnumerable<CBS.Core.HelperModels.LGAVM> LGAs { get; set; }

        public IEnumerable<DTO.EscortFormationAllocationDTO> FormationsAllocated { get; set; }

        public int NumberOfOfficersRequested { get; set; }

        public long RequestId { get; set; }

        public CanApproveEscortVM CanApprove { get; set; }
    }
}