using Parkway.CBS.Core.HelperModels;
using System;

namespace Parkway.CBS.Police.Core.DTO
{
    public class GenerateRequestWithoutOfficersUploadBatchStagingDTO
    {
        public Int64 Id { get; set; }

        public string BatchRef { get; set; }

        public TaxEntityProfileLocationVM TaxEntityProfileLocation { get; set; }

        public DateTime CreatedAt { get; set; }

        public int Status { get; set; }

        public bool HasError { get; set; }

        public bool HasGeneratedInvoice { get; set; }

        public string ErrorMessage { get; set; }
    }
}