using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Admin.VM
{
    public class GenerateEscortRequestForWithoutOfficersVM
    {
        public TaxEntityProfileLocationVM BranchDetails { get; set; }

        public EscortRequestVM EscortDetails { get; set; }

        public IEnumerable<TaxEntitySubCategoryVM> Sectors { get; set; }

        public long BatchId { get; set; }
    }
}