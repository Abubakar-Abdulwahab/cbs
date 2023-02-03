using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.VM
{
    public class DIGApprovalVM
    {
        public IEnumerable<CommandVM> TacticalSquads { get; set; }

        public IEnumerable<EscortSquadAllocationVM> AssignedTacticalSquads { get; set; }
    }
}