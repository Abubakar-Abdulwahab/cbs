using Orchard.Users.Models;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Core.Models
{
    public class ApprovalAccessRole : CBSModel
    {
        public virtual string Name { get; set; }

        public virtual UserPartRecord AddedBy { get; set; }

        public virtual UserPartRecord LastUpdatedBy { get; set; }
    }
}