using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    /// <summary>
    /// this establishes a mapping of levels and permissions
    /// for example a request level for different level will have different permissions
    /// if the request is at level 1, and the view that is being shown is at level 2
    /// we use the mapping here to get the mappings for level 1 where request level is 1 and the child level is 2
    /// so we have the list of permissions that the level 2 view can have when the request is at level 1
    /// </summary>
    public class EscortViewRubric : CBSModel
    {
        public virtual EscortProcessStageDefinition RequestLevel { get; set; }

        public virtual EscortProcessStageDefinition ChildLevel { get; set; }

        public virtual int PermissionType { get; set; }

        public virtual bool IsDeleted { get; set; }
    }

}