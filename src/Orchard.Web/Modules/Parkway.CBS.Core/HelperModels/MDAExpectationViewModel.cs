using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Parkway.CBS.Core.HelperModels
{
    public class MDAExpectationViewModel
    {
        public int NumberOfRecords { get; set; } //for pagination
        public ReportOptions Options { get; set; }
        public dynamic Pager { get; set; }

        public string MDAName { get; set; }

        public IEnumerable<ExpectationReport> ExpectationReport { get; set; }
        public IEnumerable<SelectListItem> Mdas { get; set; }

        public string FromRange { get; set; }
        public string EndRange { get; set; }

        public Int64 TotalAmount { get; set; }
        public Int64 TotalNumberOfInvoices { get; set; }

        public Int64 TotalActualIncome { get; set; }
        public Int64 TotalNumberOfInvoicesPaid { get; set; }

    }    
}