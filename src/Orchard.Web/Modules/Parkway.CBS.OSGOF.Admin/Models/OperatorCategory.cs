using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.OSGOF.Admin.Models
{
    public class OperatorCategory : CBSModel
    {
        public virtual string ShortName { get; set; }

        public virtual decimal Amount { get; set; }

        public virtual TaxEntityCategory TaxProfileCategory { get; set; }
    }
}