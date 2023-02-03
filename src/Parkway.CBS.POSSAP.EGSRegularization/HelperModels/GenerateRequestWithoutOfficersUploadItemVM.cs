namespace Parkway.CBS.POSSAP.EGSRegularization.HelperModels
{
    public class GenerateRequestWithoutOfficersUploadItemVM
    {
        public long Id { get; set; }

        public string BranchCode { get; set; }

        public int NumberOfOfficers { get; set; }

        public string NumberOfOfficersValue { get; set; }

        public string CommandCode { get; set; }

        public int CommandType { get; set; }

        public string CommandTypeValue { get; set; }

        public int DayType { get; set; }

        public string DayTypeValue { get; set; }

        public bool HasError { get; set; }

        public string ErrorMessage { get; set; }
    }
}
