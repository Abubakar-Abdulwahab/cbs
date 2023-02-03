using System;

namespace Parkway.CBS.Police.Core.DTO
{
    public class GenerateRequestWithoutOfficersUploadBatchItemsStagingDTO
    {
        public Int64 Id { get; set; }

        public string BranchCode { get; set; }

        public int NumberOfOfficers { get; set; }

        public string NumberOfOfficersValue { get; set; }

        public int CommandId { get; set; }

        public string CommandCode { get; set; }

        public string CommandName { get; set; }

        public int CommandType { get; set; }

        public string CommandTypeValue { get; set; }

        public int DayType { get; set; }

        public string DayTypeValue { get; set; }

        public bool HasError { get; set; }

        public string ErrorMessage { get; set; }
    }
}