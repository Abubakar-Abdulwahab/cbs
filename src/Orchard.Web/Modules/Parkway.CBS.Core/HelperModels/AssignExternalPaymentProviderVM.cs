using System.Collections.Generic;

namespace Parkway.CBS.Core.HelperModels
{
    public class AssignExternalPaymentProviderVM
    {
        public List<PaymentProviderVM> PaymentProviders { get; set; }

        public List<MDAVM> MDAs { get; set; }

        public List<int> ExistingMDAConstraints { get; set; }

        public List<int> ExistingRHConstraints { get; set; }

        public string SelectedPaymentProvider { get; set; }

        public int SelectedPaymentProviderParsed { get; set; }

        public string SelectedPaymentProviderName { get; set; }

        public List<int> SelectedMdas { get; set; }

        public List<int> SelectedRevenueHeads { get; set; }

        public string SelectedRhAndMdas { get; set; }

        public bool IsEdit { get; set; }

        public string MDARevenueHeadAccessRestrictionsReference { get; set; }
    }
}