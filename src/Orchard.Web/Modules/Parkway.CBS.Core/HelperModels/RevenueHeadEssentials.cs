using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Parkway.CBS.Core.Models.Enums;

namespace Parkway.CBS.Core.HelperModels
{
    public class RevenueHeadEssentials
    {
        public TaxEntityCategory TaxEntityCategory { get; set; }

        public RevenueHeadDetails RevenueHeadDetails { get; set; }

        public IEnumerable<FormControl> Controls { get; set; }

        public BillingType BillingType { get; set; }
    }
}