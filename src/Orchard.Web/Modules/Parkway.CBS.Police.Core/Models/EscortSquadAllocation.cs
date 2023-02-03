using System;
using Parkway.CBS.Core.Models;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.Models
{
    /// <summary>
    /// DIG APPROVAL (approver two)
    /// the DIG selects the unit to perform to service
    /// this request
    /// These are the group of formations
    /// </summary>
    public class EscortSquadAllocation : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual Command Command { get; set; }

        public virtual int NumberOfOfficers { get; set; }

        public virtual string StatusDescription { get; set; }

        public virtual bool Fulfilled { get; set; }

        public virtual EscortSquadAllocationGroup AllocationGroup { get; set; }

        public virtual CommandType CommandType { get; set; }

        public virtual bool IsDeleted { get; set; }

        public virtual ICollection<EscortFormationAllocation> Formations { get; set; }
    }
}