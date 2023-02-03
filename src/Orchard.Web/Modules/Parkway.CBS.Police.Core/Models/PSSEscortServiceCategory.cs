using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    public class PSSEscortServiceCategory : CBSModel
    {
        public virtual PSSEscortServiceCategory Parent { get; set; }

        public virtual string Name { get; set; }

        public virtual int MinimumRequiredOfficers { get; set; }

        public virtual bool ShowExtraFields { get; set; }

        public virtual bool IsActive { get; set; }

        public virtual bool IsDeleted { get; set; }
    }
}