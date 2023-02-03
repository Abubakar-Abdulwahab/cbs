using Orchard.Roles.Models;
using Orchard.Users.Models;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    /// <summary>
    /// Here the process flow would carry the admin user
    /// and the corresponding EscortProcessStageDefinition that is the level for this 
    /// admin. For example the AIG admin (user) would be on level 3, DIG (user) level 2, Desk officer (user) level 1
    /// <para>The constraint here is that we can only have one role per level
    /// For the Desk officer approval level we have the Name DeskOfficer approval, level would be 1</para>
    /// </summary>
    public class EscortProcessFlow : CBSModel
    {
        public virtual string Name { get; set; }

        public virtual EscortProcessStageDefinition Level { get; set; }

        /// <summary>
        /// Admin user asssigned to this level
        /// </summary>
        public virtual PSSAdminUsers AdminUser { get; set; }


        public virtual CommandType CommandType { get; set; }

        /// <summary>
        /// Added by admin user
        /// </summary>
        public virtual UserPartRecord AddedBy { get; set; }

        /// <summary>
        /// last updated by admin user
        /// </summary>
        public virtual UserPartRecord LastUpdatedBy { get; set; }

        public virtual bool IsActive { get; set; }
    }
}