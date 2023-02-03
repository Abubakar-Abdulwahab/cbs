using Orchard.Roles.Models;
using Orchard.Users.Models;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    public class PSSAdminUsers : CBSModel
    {
        public virtual string Fullname { get; set; }

        public virtual RoleRecord RoleType { get; set; }

        public virtual UserPartRecord User { get; set; }

        public virtual string PhoneNumber { get; set; }

        public virtual string Email { get; set; }

        public virtual bool IsActive { get; set; }

        public virtual CommandCategory CommandCategory { get; set; }

        public virtual Command Command { get; set; }

        public virtual UserPartRecord CreatedBy { get; set; }

        public UserPartRecord LastUpdatedBy { get; set; }
    }
}