using Parkway.CBS.Police.Core.HelperModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Core.DTO
{
    public class PSSBranchOfficersUploadBatchItemsStagingDTO
    {
        public Int64 Id { get; set; }

        public Int64 PSSBranchOfficersUploadBatchStagingId { get; set; }

        public string APNumber { get; set; }

        public CommandVM OfficerCommand { get; set; } 
    }
}