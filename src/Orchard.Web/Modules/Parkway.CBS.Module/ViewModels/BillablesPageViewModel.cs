using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Module.ViewModels
{
    public class BillablesPageViewModel
    {
        public List<RevenueHeadAndBillingInformation> RevenueHeadsAndBillings { get; set; }
        public dynamic Pager { get; set; }
        public RHIndexOptions Options { get; set; }
        public PINViewModel PINViewModel { get; set; }

        public List<string>TaxEntityCategories { get; set; }

    }
}