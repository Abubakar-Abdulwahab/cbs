using System;
using System.Collections.Generic;

namespace Parkway.CBS.Core.HelperModels
{
    public class GenerateInvoiceConfirmTaxPayer
    {
        public TaxPayerWithDetails TaxPayerWithDetails { get; set; }

        public List<RevenueHeadLite> RevenueHeads { get; set; }

        public Int64 TaxPayerId { get; set; }

        public string RevenueHeadName { get; set; }

        public string MDAName { get; set; }

    }
}