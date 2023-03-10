using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.HelperModels;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Admin.VM
{
    public class EscortRequestDetailVM
    {
        public TaxEntityProfileLocationVM BranchTaxEntityProfileLocation { get; set; }

        public IEnumerable<PSSBranchOfficersUploadBatchItemsStagingVM> BranchOfficers { get; set; }

        public FileUploadReport PSSBranchOfficerUploadBatchItemsReport { get; set; }

        public int TotalRecordCount { get; set; }

        public int Status { get; set; }

        public dynamic Pager { get; set; }

        public long ProfileId { get; set; }

        public long BatchId { get; set; }

        public string LogoURL { get; set; }

        public string TenantName { get; set; }
    }
}