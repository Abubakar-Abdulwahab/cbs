using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class PaymentProviderSearchParams
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int Take { get; set; }

        public int Skip { get; set; }

        public bool DontPageData { get; set; }
    }
}