using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.ReferenceData.Admin.ViewModels
{
    public class NAGISInvoiceSummaryVM
    {
        public IEnumerable<NAGISInvoiceSummaryCollection> ReportRecords { get; set; }
    }

    public class NAGISInvoiceSummaryCollection
    {
        public string NAGISInvoiceNumber { get; set; }

        public string InvoiceNumber { get; set; }

        public string PayerId { get; set; }
    }
}