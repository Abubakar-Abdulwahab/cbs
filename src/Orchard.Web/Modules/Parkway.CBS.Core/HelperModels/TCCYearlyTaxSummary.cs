using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class TCCYearlyTaxSummary
    {
        public int Year { get; set; }

        public decimal TotalIncome { get; set; }

        public decimal TotalTaxPaid { get; set; }
    }
}