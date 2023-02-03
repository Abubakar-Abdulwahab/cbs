using System;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    /// <summary>
    /// SQUADRON LEADER APPROVAL (approver four)
    /// </summary>
    public class EscortFormationOfficer : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual EscortFormationAllocation FormationAllocation { get; set; }

        public virtual EscortSquadAllocationGroup Group { get; set; }

        public virtual PolicerOfficerLog PoliceOfficerLog { get; set; }

        public virtual decimal EscortRankRate { get; set; }

        public bool IsDeleted { get; set; }
    }
}