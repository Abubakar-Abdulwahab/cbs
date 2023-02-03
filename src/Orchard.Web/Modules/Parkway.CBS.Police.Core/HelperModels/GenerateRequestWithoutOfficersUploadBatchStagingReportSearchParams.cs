using System;

namespace Parkway.CBS.Police.Core.HelperModels
{
    public class GenerateRequestWithoutOfficersUploadBatchStagingReportSearchParams
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int TaxEntityProfileLocationId { get; set; }

        public bool PageData { get; set; }

        public int Skip { get; set; }

        public int Take { get; set; }
    }
}