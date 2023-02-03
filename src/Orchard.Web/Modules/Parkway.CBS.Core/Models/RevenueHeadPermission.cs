using Orchard.Users.Models;

namespace Parkway.CBS.Core.Models
{
    public class RevenueHeadPermission : CBSModel
    {
        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        public virtual UserPartRecord LastUpdatedBy { get; set; }

        public virtual bool IsActive { get; set; }
    }
}