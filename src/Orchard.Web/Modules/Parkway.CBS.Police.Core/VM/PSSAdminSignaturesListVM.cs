using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.VM
{
    public class PSSAdminSignaturesListVM
    {
        public IEnumerable<PSSAdminSignatureUploadVM> Signatures { get; set; }

        public int TotalSignaturesUploaded { get; set; }

        public string Start { get; set; }

        public string End { get; set; }

        public dynamic Pager { get; set; }
    }
}