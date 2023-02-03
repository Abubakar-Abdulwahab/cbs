using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class PaymentPerRevenueReport
    {
        public string MDAName { get; set; }
        public string RevenueHeadName { get; set; }
        public string RevenueHeadSlug { get; set; }
        public int RevenueHeadId { get; set; }
        public Int64 NumberOfInvoices { get; set; }
        public Int64 ExpectedIncome { get; set; }
        public Int64 NumberOfInvoicesPaid { get; set; }
        public Int64 ActualIncome { get; set; }
    }
}