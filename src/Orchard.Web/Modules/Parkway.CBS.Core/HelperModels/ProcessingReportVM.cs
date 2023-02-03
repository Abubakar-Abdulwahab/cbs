using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using System;

namespace Parkway.CBS.Core.HelperModels
{
    public class ProcessingReportVM : InvoiceGenerationDetailsModel
    {
        public Int64 BatchId { get; set; }

        public string RequestToken { get; set; }

        public string AdapterValue { get; set; }

        public TaxEntity Entity { get; set; }

        public bool FromTaxProfileSetup { get; set; }


        /// <summary>
        /// this object would contain details you might need to move from profile generation to comfirm invoice page
        /// or paye assessment page as the case might be
        /// </summary>
        public ProceedWithInvoiceGenerationVM BackHistory { get; set; }


        public PayeAssessmentType Type { get; set; }
    }
}