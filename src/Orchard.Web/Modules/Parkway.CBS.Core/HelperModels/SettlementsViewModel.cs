using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class SettlementsViewModel
    {
        public dynamic Pager { get; set; }

        //public string SelectedMDA { get; set; }

        //public string SelectedRevenueHead { get; set; }

        //public string SelectedPaymentProvider { get; set; }

        public string RuleIdentifier { get; set; }

        public string Name { get; set; }

        public List<SettlementRuleLite> RuleRecords { get; set; }

        public long TotalRules { get; set; }

        //public List<ExternalPaymentProviderVM> PaymentProviders { get; set; }

        //public List<RevenueHeadVM> RevenueHeads { get; set; }

        //public List<MDAVM> MDAs { get; set; }
    }
}