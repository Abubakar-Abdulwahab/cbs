using System;
using Parkway.CBS.Core.Models;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.Models
{
    /// <summary>
    /// AIG APPROVAL (approver three)
    /// Here the AIG selects the formation within juridiction
    /// these are the formations that would service this request
    /// </summary>
    public class EscortFormationAllocation : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual bool IsDeleted { get; set; }

        public virtual EscortSquadAllocationGroup Group { get; set; }

        public virtual EscortSquadAllocation EscortSquadAllocation { get; set; }

        public virtual PSSAdminUsers AllocatedByAdminUser { get; set; }

        public virtual StateModel State { get; set; }

        public virtual LGA LGA { get; set; }

        public virtual Command Command { get; set; }

        public virtual int NumberOfOfficers { get; set; }

        public virtual int NumberAssignedByCommander { get; set; }

        public virtual string StatusDescription { get; set; }

        public virtual bool Fulfilled { get; set; }

        public virtual ICollection<EscortFormationOfficer> SquadronOfficers { get; set; }
    }

}