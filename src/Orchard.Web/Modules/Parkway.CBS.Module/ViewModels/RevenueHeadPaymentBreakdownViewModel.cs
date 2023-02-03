using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Module.ViewModels
{
    public class RevenueHeadPaymentBreakdownViewModel
    {
        public Int64 NumberOfRecords { get; set; }
        public ReportOptions Options { get; set; }
        public Months Months { get; set; }
        public dynamic Pager { get; set; }

        public string MDAName { get; set; }
        public string RevenueHeadName { get; set; }

        public IEnumerable<RevenueBreakDownPaymentReport> RevenueBreakDownPaymentReport { get; set; }

        public string FromRange { get; set; }
        public string EndRange { get; set; }

        public Int64 TotalAmount { get; set; }
        public Int64 TotalNumberOfInvoices { get; set; }

        public Int64 TotalActualIncome { get; set; }
        public Int64 TotalNumberOfInvoicesPaid { get; set; }
    }

    public class RevenueBreakDownPaymentReport
    {
        public int RevenueHeadId { get; set; }
        public Int64 AmountPaid { get; set; }
        public string Date { get; set; }
    }
}