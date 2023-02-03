using Parkway.CBS.Core.HelperModels;
using System;

namespace Parkway.CBS.Police.Core.DTO
{
    public class PSSBranchSubUsersUploadBatchStagingDTO
    {
        public virtual Int64 Id { get; set; }

        public CBSUserVM CBSUser { get; set; }

        public TaxEntityViewModel TaxEntity { get; set; }

        public string BatchRef { get; set; }

        public int Status { get; set; }

        public bool HasError { get; set; }

        public string ErrorMessage { get; set; }
    }
}