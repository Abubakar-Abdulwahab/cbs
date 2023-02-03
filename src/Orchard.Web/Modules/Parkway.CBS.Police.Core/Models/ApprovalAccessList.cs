using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Models
{
    public class ApprovalAccessList : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual CommandCategory CommandCategory { get; set; }

        public virtual StateModel State { get; set; }

        public virtual LGA LGA { get; set; }

        public virtual Command Command { get; set; }

        public virtual PSService Service { get; set; }

        public virtual ApprovalAccessRoleUser ApprovalAccessRoleUser { get; set; }

        public virtual bool IsDeleted { get; set; }
    }
}