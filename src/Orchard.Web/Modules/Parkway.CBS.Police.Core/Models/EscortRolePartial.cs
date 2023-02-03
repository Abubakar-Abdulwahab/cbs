using Orchard.Roles.Models;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    public class EscortRolePartial : CBSModel
    {
        public virtual RoleRecord Role { get; set; }

        public virtual string ImplementationClass { get; set; }

        public virtual string PartialName { get; set; }

        public virtual bool IsActive { get; set; }
    }
}