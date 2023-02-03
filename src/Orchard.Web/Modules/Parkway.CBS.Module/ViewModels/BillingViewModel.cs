using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.FileUpload;

namespace Parkway.CBS.Module.ViewModels
{
    public class BillingViewModel
    {
        public DirectAssessmentVM DirectAssessment { get; set; }

        public DueDateModel DueDateModel { get; set; }

        public string SAmount { get; set; }

        public string Surcharge { get; set; }

        public bool IsRecurring { get; set; }

        public bool IsPrepaid { get; set; }

        public bool IsDirectAssessment { get; set; }

        public BillingType BillingType { get; set; }

        public BillingFrequencyModel FrequencyModel { get; set; }

        public string RHName { get; set; }

        public DiscountModel DiscountModel { get; set; }

        public PenaltyModel PenaltyModel { get; set; }

        public int Indexer { get; set; }

        public int IndexerPenalty { get; set; }

        public BillingDemandNotice BillingDemandNotice { get; set; }

        //for postback matters
        public IEnumerable<DiscountModel> DiscountModelPostBackData { get; set; }

        public IEnumerable<PenaltyModel> PenaltyModelPostBackData { get; set; }

        public CallBackObject CallBackObject { get; set; }

        public FileUploadTemplatesVM FileUploadBillingModel { get; set; }

        public bool IsEdit { get; set; }
}

    public class CallBackObject
    {
        public bool HasFrequencyValue { get; set; }
    }
}