using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Core.HelperModels
{
    public class GenerateRequestWithoutOfficersUploadBatchItemsStagingReportSearchParams
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public Int64 GenerateRequestWithoutOfficersUploadBatchStagingId { get; set; }

        public int ProfileId { get; set; }

        public bool PageData { get; set; }

        public int Skip { get; set; }

        public int Take { get; set; }
    }
}