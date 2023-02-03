using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Models
{
    public class RevenueHeadGroup : CBSModel
    {
        public virtual RevenueHead GroupParent { get; set; }

        public virtual RevenueHead RevenueHead { get; set; }
    }
}