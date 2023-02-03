using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Models
{
    public class RefDataDescription : CBSModel
    {
        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        public virtual string ImplementingClass { get; set; }

        public virtual bool IsActive { get; set; }
    }
}