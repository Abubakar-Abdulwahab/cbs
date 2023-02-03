using Orchard.Users.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Models
{
    public class CBSUserRole : CBSModel
    {
        public virtual UserPartRecord User { get; set; }

        public virtual CBSRole Role { get; set; }

        public virtual UserPartRecord LastUpdatedBy { get; set; }
    }
}