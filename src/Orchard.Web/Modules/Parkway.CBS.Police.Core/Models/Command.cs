using Orchard.Users.Models;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    public class Command : CBSModel
    {
        public virtual string Name { get; set; }

        public virtual string Code { get; set; }

        public virtual CommandCategory CommandCategory { get; set; }

        public virtual LGA LGA { get; set; }

        public virtual StateModel State { get; set; }

        /// <summary>
        /// Added by admin user
        /// </summary>
        public virtual UserPartRecord AddedBy { get; set; }

        /// <summary>
        /// last updated by admin user
        /// </summary>
        public virtual UserPartRecord LastUpdatedBy { get; set; }

        public virtual bool IsActive { get; set; }

        public virtual string Address { get; set; }

        public virtual CommandType CommandType { get; set; }

        public virtual string ParentCode { get; set; }

        public virtual Command ZonalCommand { get; set; }

    }
}