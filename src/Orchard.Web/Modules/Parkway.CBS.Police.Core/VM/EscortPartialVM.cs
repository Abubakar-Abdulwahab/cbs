using System;

namespace Parkway.CBS.Police.Core.VM
{
    public class EscortPartialVM
    {
        public string PartialName { get; set; }

        public dynamic PartialModel { get; internal set; }

        public string ImplementationClass { get; set; }

        public long RequestId { get; set; }

        public int UserId { get; set; }

        public Int64 AllocationId { get; set; }

        public Int64 SquadAllocationGroup { get; set; }

        public int CommandTypeId { get; set; }
    }
}