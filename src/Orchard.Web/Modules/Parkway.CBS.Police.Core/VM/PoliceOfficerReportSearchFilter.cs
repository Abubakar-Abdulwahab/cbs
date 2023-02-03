namespace Parkway.CBS.Police.Core.VM
{
    public class PoliceOfficerReportSearchFilter
    {
        public string StateCode { get; set; }

        public string LGACode { get; set; }

        public string CommandCode { get; set; }

        public string RankCode { get; set; }

        public string GenderCode { get; set; }

        public string ServiceNumber { get; set; }

        public string Name { get; set; }

        public string IPPISNumber { get; set; }

        public string Page { get; set; }

        public string PageSize { get; set; }

        public string TotalNumberOfOfficers { get; set; }
    }
}