using Orchard.Users.Models;

namespace Parkway.CBS.Core.Models
{
    public class CBSRolePermission : CBSModel
    {
        public virtual CBSRole Role { get; set; }

        public virtual CBSPermission Permission { get; set; }

        public virtual UserPartRecord LastUpdatedBy { get; set; }
    }
}