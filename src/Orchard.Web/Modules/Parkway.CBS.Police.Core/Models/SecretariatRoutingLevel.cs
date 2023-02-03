using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Models
{
    public class SecretariatRoutingLevel : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual PSSRequest Request { get; set; }

        public virtual int StageRoutedTo { get; set; }

        public virtual string StageModelName { get; set; }

        public virtual PSSAdminUsers AdminUser { get; set; }
    }
}