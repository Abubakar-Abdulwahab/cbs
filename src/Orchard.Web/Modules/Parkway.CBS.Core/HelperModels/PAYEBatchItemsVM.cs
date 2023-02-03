using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class PAYEBatchItemsVM
    {
        public decimal GrossAnnual { get; set; }

        public decimal Exemptions { get; set; }

        public decimal IncomeTaxPerMonth { get; set; }

        public int MonthId { get; set; }

        public string Month => ((Models.Enums.Months)MonthId).ToString();

        public int Year { get; set; }

        public string PayerName { get; set; }

        public string PayerId { get; set; }
    }
}