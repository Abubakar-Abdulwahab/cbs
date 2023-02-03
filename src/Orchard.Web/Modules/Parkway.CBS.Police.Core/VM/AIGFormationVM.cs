using System;

namespace Parkway.CBS.Police.Core.VM
{
    public class AIGFormationVM
    {
        public int StateId { get; set; }

        public int LGAId { get; set; }

        public int FormationId { get; set; }

        public string FormationName { get; set; }

        public int NumberofOfficers { get; set; }

        public long RequestId { get; set; }

        public DateTime DateCreated { get; set; }

        public int NumberOfOfficersProvided { get; set; }
    }
}