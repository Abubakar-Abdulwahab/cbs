using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class PAYEBatchItemReceiptViewModel
    {
        public string ReceiptNumber { get; set; }

        public string TaxPayerName { get; set; }

        public string TaxPayerEmail { get; set; }

        public string TaxPayePhoneNumber { get; set; }

        public string PaymentDate { get; set; }

        public string ReceiptLogoPath { get; set; }

        public string ReceiptLogoURL { get; set; }

        public string ShortStrip { get; set; }

        public string ReceiptBackgroundWatermarkPath { get; set; }

        public string TaxPayerTIN { get; set; }

        public string TaxPayerAddress { get; set; }

        public string TaxPayerId { get; set; }

        public decimal GrossAnnual { get; set; }

        public Months Month { get; set; }

        public int Year { get; set; }

        public string BarCodeSavingPath { get; set; }

        public decimal TaxAmountPaid { get; set; }

        public string TaxAmountPaidInWordsFirstLine { get; set; }

        public string TaxAmountPaidInWordsSecondLine { get; set; }

        public string TenantName { get; set; }
    }
}