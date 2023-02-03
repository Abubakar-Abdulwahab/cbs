using System;

namespace Parkway.CBS.Core.HelperModels
{
    public class CBSUserTaxEntityProfileLocationReportSearchParams
    {
        public long TaxEntityId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int Branch { get; set; }

        public string SubUserName { get; set; }

        public bool DontPageData { get; set; }

        public int Skip { get; set; }

        public int Take { get; set; }
    }
}