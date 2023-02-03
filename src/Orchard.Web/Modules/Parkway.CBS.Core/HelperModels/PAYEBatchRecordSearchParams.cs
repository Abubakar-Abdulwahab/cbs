using System;

namespace Parkway.CBS.Core.HelperModels
{
    public class PAYEBatchRecordSearchParams
    {
        public long TaxEntityId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string BatchRef { get; set; }

        public string datefilter { get; set; }

        public bool DontPageData { get; set; }

        public int Skip { get; set; }

        public int Take { get; set; }
    }
}