using System;
using Parkway.CBS.Core.Models;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.Models
{
    /// <summary>
    /// DIG APPROVAL INTERFACE (approver two)
    /// </summary>
    public class EscortSquadAllocationGroup : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        /// <summary>
        /// this is the level the application is on
        /// </summary>
        public virtual EscortProcessStageDefinition RequestLevel { get; set; }

        public virtual string Comment { get; set; }

        public virtual PSService Service { get; set; }

        public virtual PSSRequest Request { get; set; }

        public virtual PSSAdminUsers AdminUser { get; set; }

        public virtual bool Fulfilled { get; set; }

        public virtual string StatusDescription { get; set; }

        public virtual ICollection<EscortSquadAllocation> Allocations { get; set; }

        public virtual ICollection<EscortFormationOfficer> AssignedFormationOfficers { get; set; }
    }
}