using Newtonsoft.Json;
using Parkway.Cashflow.Ng.Models;
using Parkway.CBS.Core.Models;
using System.Collections.Generic;

namespace Parkway.CBS.Core.HelperModels
{
    public class TaxEntityInvoice
    {
        public TaxEntity TaxEntity { get; set; }

        public decimal Amount { get; set; }

        public string InvoiceDescription { get; set; }

        public List<AdditionalDetails> AdditionalDetails { get; set; }

        public int CategoryId { get; set; }

        [JsonIgnore]
        public CashFlowInvoice CashFlowInvoice { get; set; }

        public int LGAId { get; set; }
    }
}