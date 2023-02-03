using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Models
{
    public class LGA : CBSModel
    {
        public virtual string Name { get; set; }

        public virtual bool IsActive { get; set; }

        public virtual string CodeName { get; set; }

        public virtual StateModel State { get; set; }
    }
}