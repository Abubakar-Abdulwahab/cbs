using Orchard.Users.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Models
{
    public class CBSRole : CBSModel
    {
        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        public virtual UserPartRecord LastUpdatedBy { get; set; }

        public virtual bool IsActive { get; set; }

        public virtual ICollection<CBSPermission> Permissions { get; set; }
    }
}