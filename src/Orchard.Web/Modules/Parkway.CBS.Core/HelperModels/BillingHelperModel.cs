using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Parkway.CBS.Core.Models.Enums;
using System.ComponentModel.DataAnnotations;
using Parkway.CBS.Core.HTTP.Handlers.Billing;
using Parkway.CBS.FileUpload;

namespace Parkway.CBS.Core.HelperModels
{
    public class BillingHelperModel
    {
        [Required(ErrorMessage ="The authorized user email is required")]
        public string UserEmail { get; set; }
        public int RevenueHeadID { get; set; }
        public AssessmentModel AssessmentModel { get; set; }
        public decimal Surcharge { get; set; }
        public DueDateModel DueDateModel { get; set; }
        public ICollection<DiscountModel> DiscountCollection { get; set; }
        public ICollection<PenaltyModel> PenaltyCollection { get; set; }
        public BillingFrequencyModel BillingFrequencyModel { get; set; }
        public BillingType BillingType { get; set; }
        public BillingDemandNotice DemandNotice { get; set; }

        public DirectAssessmentModel DirectAssessment { get; set; }
        
        public string RequestReference { get; set; }

        public FileUploadTemplates FileUploadTemplates { get; set; }
    }
}