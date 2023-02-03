using Parkway.CBS.Core.Models.Enums;
using System;

namespace Parkway.CBS.Core.HelperModels
{
    public class PaymentReferenceVM
    {
        public string ReferenceNumber { get; set; }

        public string InvoiceNumber { get; set; }

        public long Id { get; set; }

        public PaymentProvider PaymentProvider { get; set; }

        public DateTime DateGenerated { get; set; }

        public string InvoiceDescription { get; set; }

        public string PayerId { get; set; }

        public string Recipient { get; set; }

        public string RevenueHead { get; set; }
    }
}