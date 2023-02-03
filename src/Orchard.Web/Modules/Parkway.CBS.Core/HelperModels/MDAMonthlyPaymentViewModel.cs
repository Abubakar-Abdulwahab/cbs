using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class MDAMonthlyPaymentViewModel
    {
        public Int64 NumberOfRecords { get; set; }
        public ReportOptions Options { get; set; }
        public Months Months { get; set; }
        public dynamic Pager { get; set; }
        public IEnumerable<PaymentReport> PaymentReport { get; set; }

        public string FromRange { get; set; }
        public string EndRange { get; set; }

        public Int64 TotalAmount { get; set; }
        public Int64 TotalNumberOfInvoices { get; set; }

        public Int64 TotalActualIncome { get; set; }
        public Int64 TotalNumberOfInvoicesPaid { get; set; }
    }
}