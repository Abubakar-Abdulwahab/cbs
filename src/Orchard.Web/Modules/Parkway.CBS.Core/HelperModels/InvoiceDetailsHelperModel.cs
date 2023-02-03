using System;
using System.Collections.Generic;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Core.HelperModels
{
    public class InvoiceDetailsHelperModel
    {
        public Invoice Invoice { get; set; }

        public Int64 TaxEntityId { get; set; }

        public TaxEntityAccount TaxEntityAccount { get; set; }

        public int TaxCategoryId { get; set; }

        public int RevenueHeadId { get; set; }

        public int MDAId { get; set; }

        public decimal AmountDue { get; set; }

        public string ExpertSystemClientSecret { get; set; }

        public string APIRequestReference { get; set; }

        public List<InvoiceItemsSummary> InvoiceItems { get; set; }

        public string MDAName { get; set; }

        public string RevenueHead { get; set; }
    }
}