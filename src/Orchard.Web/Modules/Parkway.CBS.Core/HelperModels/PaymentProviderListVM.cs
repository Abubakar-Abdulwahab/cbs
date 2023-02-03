using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class PaymentProviderListVM
    {
        public List<ExternalPaymentProviderVM> PaymentProviders { get; set; }

        public int TotalProviders { get; set; }

        public dynamic Pager { get; set; }

        public string To { get; set; }

        public string From { get; set; }
    }
}