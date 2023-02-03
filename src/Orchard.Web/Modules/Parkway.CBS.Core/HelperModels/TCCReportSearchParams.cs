using Parkway.CBS.Core.Models.Enums;
using System;

namespace Parkway.CBS.Core.HelperModels
{
    public class TCCReportSearchParams
    {
        public long TaxEntityId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string ApplicationNumber { get; set; }

        public TCCRequestStatus SelectedStatus { get; set; }

        public string PayerId { get; set; }

        public string ApplicantName { get; set; }

        public bool DontPageData { get; set; }

        public int Skip { get; set; }

        public int Take { get; set; }

        public int ApprovalLevelId { get; set; }

    }
}