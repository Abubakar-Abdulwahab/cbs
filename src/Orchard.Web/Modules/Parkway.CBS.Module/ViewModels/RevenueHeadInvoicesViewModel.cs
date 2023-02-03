using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Module.ViewModels
{
    public class RevenueHeadInvoicesViewModel
    {
        public string RevenueHeaadName { get; set; }
        public Int64 NumberOfRecords { get; set; }
        public ReportOptions Options { get; set; }
        public dynamic Pager { get; set; }
        public IEnumerable<InvoicesReport> RevenueReport { get; set; }

        public string FromRange { get; set; }
        public string EndRange { get; set; }

        public Int64 TotalAmount { get; set; }
        public Int64 TotalNumberOfInvoices { get; set; }
    }

    public class InvoicesReport
    {
        public string MDAName { get; set; }
        public string RevenueHeadName { get; set; }
        public Int64 AmountPaid { get; set; }
        public string Date { get; set; }
        public string InvoiceNumber { get; set; }
    }
}