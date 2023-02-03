using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class PaymentReport
    {
        public string MDAName { get; set; }
        public string MDASlug { get; set; }
        public Int64 ExpectedIncome { get; set; }
        public Int64 ActualIncome { get; set; }
        public Int64 NumberOfInvoices { get; set; }
    }
}