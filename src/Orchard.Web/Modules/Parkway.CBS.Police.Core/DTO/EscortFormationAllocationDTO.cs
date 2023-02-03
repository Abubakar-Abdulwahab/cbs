using System;

namespace Parkway.CBS.Police.Core.DTO
{
    public class EscortFormationAllocationDTO
    {
        public Int64 Id { get; set; }

        public int StateId { get; set; }

        public string StateName { get; set; }

        public int LGAId { get; set; }

        public string LGAName { get; set; }

        public int FormationId { get; set; }

        public string FormationName { get; set; }

        public int NumberOfOfficers { get; set; }

        public int NumberOfOfficersAssignedByCommander { get; set; }

        public Int64 AllocationGroupId { get; set; }
    }
}