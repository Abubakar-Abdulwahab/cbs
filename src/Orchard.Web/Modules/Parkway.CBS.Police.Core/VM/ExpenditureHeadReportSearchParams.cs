namespace Parkway.CBS.Police.Core.VM
{
    public class ExpenditureHeadReportSearchParams
    {
        public string ExpenditureHeadName { get; set; }

        public string Code { get; set; }

        public int Status { get; set; }

        public bool DontPageData { get; set; }

        public int Skip { get; set; }

        public int Take { get; set; }
    }
}