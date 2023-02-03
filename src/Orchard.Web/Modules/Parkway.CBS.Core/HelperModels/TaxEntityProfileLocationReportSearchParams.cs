using System;

namespace Parkway.CBS.Core.HelperModels
{
    public class TaxEntityProfileLocationReportSearchParams
    {
        public long TaxEntityId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Address { get; set; }

        public int State { get; set; }

        public int LGA { get; set; }

        public string Name { get; set; }

        public bool DontPageData { get; set; }

        public int Skip { get; set; }

        public int Take { get; set; }
    }
}