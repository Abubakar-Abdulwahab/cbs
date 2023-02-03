using Orchard.Users.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Models
{
    public class AuditTrail : CBSModel
    {
        public virtual int Type { get; set; }
        public virtual string Model{ get; set; } 
        public virtual int Source_Id { get; set; }
        public virtual UserPartRecord AddedBy { get; set; }

    }
}