using Orchard.Users.Models;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    public class ApprovalAccessRoleUser : CBSModel
    {
        public virtual UserPartRecord User { get; set; }

        public virtual bool IsActive { get; set; }

        public virtual UserPartRecord AddedBy { get; set; }

        /// <summary>
        /// <see cref="Enums.AdminUserType"/>
        /// </summary>
        public virtual int AccessType { get; set; }
    }
}