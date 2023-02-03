using System;

namespace Parkway.CBS.Police.Core.DTO
{
    public class PSSBranchSubUsersUploadBatchItemsStagingDTO
    {
        public Int64 Id { get; set; }

        public  string BranchStateValue { get; set; }

        public  string BranchLGAValue { get; set; }

        public  string BranchAddress { get; set; }

        public  string BranchName { get; set; }

        public  string SubUserName { get; set; }

        public  string SubUserEmail { get; set; }

        public  string SubUserPhoneNumber { get; set; }

        public  bool HasError { get; set; }

        public  string ErrorMessage { get; set; }

        public int UserId { get; set; }
    }
}