using Orchard.Users.Models;

namespace Parkway.CBS.Core.Models
{
    public class AccessRole : CBSModel
    {
        public virtual string Name { get; set; }

        public virtual UserPartRecord AddedBy { get; set; }

        public virtual UserPartRecord LastUpdatedBy { get; set; }

        public virtual bool IsActive { get; set; }

        /// <summary>
        /// <see cref="Enums.AccessType"/>
        /// </summary>
        public virtual int AccessType { get; set; }
    }
}