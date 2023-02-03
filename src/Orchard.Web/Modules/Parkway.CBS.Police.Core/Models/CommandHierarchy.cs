using Orchard.Users.Models;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    public class CommandHierarchy : CBSModel
    {
        public virtual string GroupName { get; set; }

        /// <summary>
        /// Group Id
        /// </summary>
        public virtual int GroupId { get; set; }

        /// <summary>
        /// Added by admin user
        /// </summary>
        public virtual UserPartRecord AddedBy { get; set; }

        /// <summary>
        /// last updated by admin user
        /// </summary>
        public virtual UserPartRecord LastUpdatedBy { get; set; }
    }
}