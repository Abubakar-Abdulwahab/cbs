using Parkway.CBS.Core.Models.Enums;
using System;

namespace Parkway.CBS.Core.HelperModels
{
    public class PAYEBatchRecordVM
    {
        public Int64 BatchRecordId { get; set; }

        public string BatchRef { get; set; }

        public decimal TotalIncomeTaxForPayesInSchedule { get; set; }

        public decimal RevenueHeadSurCharge { get; set; }

        public bool PaymentCompleted { get; set; }

        public PayeAssessmentType AssessmentType { get; set; }

        public string PaymentTypeCode { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}