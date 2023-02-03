using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Models
{
    public class EscortFormationOfficerSelectionTracking : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual EscortFormationAllocation FormationAllocation { get; set; }

        public virtual EscortSquadAllocationGroup Group { get; set; }

        public virtual PolicerOfficerLog PoliceOfficerLog { get; set; }

        public virtual decimal EscortRankRate { get; set; }

        public virtual string Reference { get; set; }

        public bool IsDeleted { get; set; }
    }
}