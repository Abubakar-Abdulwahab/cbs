using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Core.VM
{
    public class ReceiptDetailsVM
    {
        public string Recipient { get; set; }

        public string PayerId { get; set; }

        public string ServiceName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string TIN { get; set; }

        public string Address { get; set; }

        public string FileNumber { get; set; }

        public string InvoiceDesc { get; set; }

        public List<InvoicePaymentsVM> Transactions { get; set; }

        public decimal AmountDue { get; set; }

        public decimal TotalAmountPaid { get { return Transactions.Sum(r => r.AmountPaid); } }

        public string InvoiceNumber { get; set; }

        public HeaderObj HeaderObj { get; set; }

        public string ReceiptLogoPath { get; set; }

        public string ReceiptLogoURL { get; set; }

        public string ShortStrip { get; set; }

        public string LongStrip { get; set; }

        public string TenantName { get; set; }

        public string BarCodeSavingPath { get; set; }

        public string ReceiptNumber { get { return Transactions.First().ReceiptNumber; } }

        public string PaymentDate { get { return Transactions.First().ReceiptNumber; } }

    }
}