using Orchard.Users.Models;

namespace Parkway.CBS.Core.Models
{
    public class RevenueHeadPermissionConstraints : CBSModel
    {
        public virtual RevenueHeadPermission RevenueHeadPermission { get; set; }

        public virtual MDA MDA { get; set; }

        public virtual RevenueHead RevenueHead { get; set; }

        public virtual ExpertSystemSettings ExpertSystem { get; set; }

        public virtual UserPartRecord LastUpdatedBy { get; set; }
    }
}