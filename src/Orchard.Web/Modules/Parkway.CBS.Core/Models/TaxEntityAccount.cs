using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Models
{
    public class TaxEntityAccount : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual decimal Credits { get; set; }

        public virtual decimal Debits { get; set; }
    }
}