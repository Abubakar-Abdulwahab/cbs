using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.Models.Enums;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.VM
{
    public class ExtractDetailsVM
    {
        public ExtractRequestVM ExtractInfo { get; set; }

        public TaxEntityViewModel TaxEntity { get; set; }

        public List<PSSRequestStatusLogVM> RequestStatusLog { get; set; }

        public string FileRefNumber { get; set; }

        public string ApprovalNumber { get; set; }

        public int RequestStatus { get; set; }
    }
}