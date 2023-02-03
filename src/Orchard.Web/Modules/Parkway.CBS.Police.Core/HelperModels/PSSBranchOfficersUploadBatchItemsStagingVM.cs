using System;

namespace Parkway.CBS.Police.Core.HelperModels
{
    public class PSSBranchOfficersUploadBatchItemsStagingVM
    {
        public long Id { get; set; }

        public string BatchReference { get; set; }

        /// <summary>
        /// <see cref="Enums.PSSBranchOfficersUploadStatus"/>
        /// </summary>
        public int Status { get; set; }

        public bool HasError { get; set; }

        public string ErrorMessage { get; set; }

        public string ServiceNumber { get; set; }

        public string Name { get; set; }

        public string Command { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}