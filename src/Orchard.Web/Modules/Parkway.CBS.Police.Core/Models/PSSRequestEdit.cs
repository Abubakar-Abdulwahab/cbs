using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Models
{
    public class PSSRequestEdit : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual PSSRequest Request { get; set; }

        public virtual RequestCommandWorkFlowLog RoutedAtRequestCommandWorkFlowLog { get; set; }

        public virtual PSSAdminUsers AdminUser { get; set; }

        public virtual bool IsActive { get; set; }
        
        public virtual string RequestEditReference { get; set; }
    }
}