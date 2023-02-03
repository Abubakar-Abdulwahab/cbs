using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Parkway.CBS.Module.ViewModels
{
    public class PerRevenueReportViewModel
    {
        public Int64 NumberOfRecords { get; set; }
        public ReportOptions Options { get; set; }
        public Months Months { get; set; }
        public dynamic Pager { get; set; }
        public IEnumerable<RevenueReport> RevenueReport { get; set; }

        public IEnumerable<SelectListItem> Mdas { get; set; }

        public string FromRange { get; set; }
        public string EndRange { get; set; }

        public Int64 TotalAmount { get; set; }
        public Int64 TotalNumberOfInvoices { get; set; }

        public string MDAName { get; set; }
    }

    public class RevenueReport
    {
        public string MDAName { get; set; }
        public string RevenueHeadName { get; set; }
        public Int64 NumberOfInvoices { get; set; }
        public Int64 AmountPaid { get; set; }
        public string Slug { get; set; }
        public int Id { get; set; }
    }
}