using System;

namespace Parkway.CBS.Police.Core.HelperModels
{
    public class PSSBranchOfficersUploadBatchStagingVM
    {
        public long Id { get; set; }

        public int TaxProfileLocationId { get; set; }

        public string BatchReference { get; set; }

        /// <summary>
        /// <see cref="Enums.PSSBranchOfficersUploadStatus"/>
        /// </summary>
        public int Status { get; set; }

        public bool HasError { get; set; }

        public string ErrorMessage { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool HasGeneratedInvoice { get; set; }
    }
}