using Orchard.Users.Models;
using Parkway.CBS.Core.Models;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.Models
{
    /// <summary>
    /// Stage definition is where we list the stages/states the escort application 
    /// can be in, so for example the state can be pedning DIG approval
    /// </summary>
    public class EscortProcessStageDefinition : CBSModel
    {
        public virtual string Name { get; set; }

        public virtual string StageDescription { get; set; }

        public virtual EscortProcessStageDefinition ParentDefinition { get; set; }

        /// <summary>
        /// Added by admin user
        /// </summary>
        public virtual UserPartRecord AddedBy { get; set; }

        /// <summary>
        /// last updated by admin user
        /// </summary>
        public virtual UserPartRecord LastUpdatedBy { get; set; }

        public virtual bool IsActive { get; set; }

        public virtual CommandType CommandType { get; set; }

        public virtual int LevelGroupIdentifier { get; set; }

        public virtual ICollection<EscortViewRubric> Rubric { get; set; }

        public virtual ICollection<EscortProcessFlow> AssignedFlow { get; set; }

    }
}