using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    public class EscortFormationAllocationSelectionTracking : CBSBaseModel
    {
        public virtual long Id { get; set; }

        public virtual bool IsDeleted { get; set; }

        public virtual EscortSquadAllocationGroup Group { get; set; }

        public virtual EscortSquadAllocation EscortSquadAllocation { get; set; }

        public virtual PSSAdminUsers AllocatedByAdminUser { get; set; }

        public virtual StateModel State { get; set; }

        public virtual LGA LGA { get; set; }

        public virtual Command Command { get; set; }

        public virtual int NumberOfOfficers { get; set; }

        public virtual string Reference { get; set; }
    }

}