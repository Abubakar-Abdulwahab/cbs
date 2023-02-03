using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Models
{
    public class EscortSquadAllocationSelectionTracking : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual Command Command { get; set; }

        public virtual int NumberOfOfficers { get; set; }

        public virtual bool IsDeleted { get; set; }

        public virtual EscortSquadAllocationGroup AllocationGroup { get; set; }

        public virtual string Reference { get; set; }
    }
}