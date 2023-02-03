using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.VM
{
    public class AssignEscortProcessFlowVM
    {
        public IEnumerable<EscortProcessFlowVM> EscortProcessFlows { get; set; }

        public IEnumerable<CommandTypeVM> CommandTypes { get; set; }
    }
}