using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Models
{
    public class ScheduleLog : CBSBaseModel
    {
        public BillingSchedule BillingSchedule { get; set; }

        public decimal Amount { get; set; }

        public BillingModel BillingModel { get; set; }

        public string BillingSnap { get; set; }

        public RevenueHead RevenueHead { get; set; }

        public string InvoiceNumber { get; set; }

        public string TaxPayerNumber { get; set; }

    }
}