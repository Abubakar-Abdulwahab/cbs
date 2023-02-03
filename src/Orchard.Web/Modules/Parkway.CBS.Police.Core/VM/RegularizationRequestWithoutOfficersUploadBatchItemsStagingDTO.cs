using System;

namespace Parkway.CBS.Police.Core.VM
{
    public class RegularizationRequestWithoutOfficersUploadBatchItemsStagingDTO
    {
        public Int64 BatchItemStagingId { get; set; }

        public decimal DeploymentRate { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int NumberOfOfficers { get; set; }

        public int CommandId { get; set; }
    }
}