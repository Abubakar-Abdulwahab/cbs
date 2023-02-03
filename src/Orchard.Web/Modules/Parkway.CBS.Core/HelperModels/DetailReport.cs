using System;
using Orchard.Users.Models;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;

namespace Parkway.CBS.Core.HelperModels
{
    public class DetailReport
    {
        public DateTime InvoiceDate { get; set; }

        public string MDAName { get; set; }

        public string RevenueHeadName { get; set; }

        public string PayersTIN { get; set; }

        public string InvoiceNumber { get; set; }

        public decimal TotalAmount { get; set; }

        public int Quantity { get; set; }
        
        public decimal AmountDue { get; set; }

        public InvoiceStatus PaymentStatus { get; set; }
        
        public DateTime? PaymentDate { get; set; }

        public DateTime? DueDate { get; set; }

        public string AdminUserName { get; set; }

        public string TaxPayerName { get; set; }
    }
}