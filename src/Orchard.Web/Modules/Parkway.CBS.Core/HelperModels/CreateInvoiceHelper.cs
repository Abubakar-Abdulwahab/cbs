using Parkway.Cashflow.Ng.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Core.HelperModels
{
    public class CreateInvoiceHelper
    {
        public DateTime InvoiceDate { get; set; }

        public decimal Amount { get; set; }

        public DiscountModel DiscountModel { get; set; }

        public DateTime DueDate { get; set; }

        public string Title { get; set; }

        public string Type { get; set; }

        public string FootNotes { get; set; }

        public List<CashFlowCreateInvoice.CashFlowProductModel> Items { get; set; }

        public string UniqueInvoiceIdentifier { get; set; }

        public string ExternalRef { get; set; }

        public string InvoiceDescription { get; set; }

        public decimal VAT { get; set; }
             
    }
}