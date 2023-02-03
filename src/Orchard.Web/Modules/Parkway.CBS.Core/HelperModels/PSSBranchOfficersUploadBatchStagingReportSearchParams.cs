namespace Parkway.CBS.Core.HelperModels
{
    public class PSSBranchOfficersUploadBatchStagingReportSearchParams
    {
        public int ProfileLocationId { get; set; }

        public bool DontPageData { get; set; }

        public int Skip { get; set; }

        public int Take { get; set; }
    }
}