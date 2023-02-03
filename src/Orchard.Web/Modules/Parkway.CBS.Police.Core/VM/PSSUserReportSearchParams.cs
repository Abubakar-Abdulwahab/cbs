using System;

namespace Parkway.CBS.Police.Core.VM
{
    public class PSSUserReportSearchParams
    {
        public bool DontPageData { get; set; }

        public int Skip { get; set; }

        public int Take { get; set; }

        public string Name { get; set; }

        public string IdentificationNumber { get; set; }

        public string UserName { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}