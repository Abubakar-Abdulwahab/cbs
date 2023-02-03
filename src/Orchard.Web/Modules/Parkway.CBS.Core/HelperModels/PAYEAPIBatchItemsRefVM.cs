using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class PAYEAPIBatchItemsRefVM
    {
        public string PayerId { get; set; }

        public string GrossAnnualEarning { get; set; }

        public decimal IncomeTaxPerMonth { get; set; }

        public string Exemptions { get; set; }

        public string Month { get; set; }

        public string Year { get; set; }

        public Int64 PAYEBatchItemsStagingId { get; set; }

        public string Mac { get; set; }

        public bool HasError { get; set; }

        public string ErrorMessages { get; set; }
    }
}