using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class ReceiptViewModel
    {
        public string ReceiptNumber { get; set; }

        public string TaxPayerName { get; set; }

        public string TaxPayerEmail { get; set; }

        public string TaxPayePhoneNumber { get; set; }

        public string PaymentDate { get; set; }

        public string AmountPaid { get; set; }

        public string ReceiptLogoPath { get; set; }

        public string ReceiptLogoURL { get; set; }

        public string ShortStrip { get; set; }

        public string TaxPayerTIN { get; set; }

        public string TaxPayerAddress { get; set; }

        public string TaxPayerId { get; set; }

        public InvoiceStatus InvoiceStatus { get; set; }

        public string InvoiceNumber { get; set; }

        public decimal AmountDue { get; set; }

        public string ExternalRef { get; set; }

        public string BarCodeSavingPath { get; set; }

        public decimal TotalAmountPaid
        {
            get
            {
                return this.TransactionLogInvoiceDetails.Sum(r => r.AmountPaid);
            }
        }

        public IEnumerable<TransactionLogInvoiceDetails> TransactionLogInvoiceDetails { get; set; }

        public string LongStrip { get; set; }

        public string TenantName { get; set; }

        public string TenantNameSuffix { get; set; }

        public string MDAName { get; set; }
    }
}