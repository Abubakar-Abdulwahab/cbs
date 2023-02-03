namespace Parkway.CBS.Police.Core.VM
{
    public class AdminUserReportSearchParams
    {
        public bool DontPageData { get; set; }

        public int Skip { get; set; }

        public int Take { get; set; }

        public int CommandCategoryId { get; set; }

        public int CommandId { get; set; }

        public int RoleType { get; set; }

        public int Status { get; set; }

        public string Username { get; set; }
    }
}