namespace Parkway.CBS.Core.HelperModels
{
    public class PAYEBatchRecordStagingVM
    {
        public bool ErrorOccurred { get; set; }

        public string ErrorMessage { get; set; }

        public decimal PercentageProgress { get; set; }
    }
}