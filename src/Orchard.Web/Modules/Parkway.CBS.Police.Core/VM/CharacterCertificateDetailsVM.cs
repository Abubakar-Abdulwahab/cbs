using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.Models.Enums;
using System;

namespace Parkway.CBS.Police.Core.VM
{
    public class CharacterCertificateDetailsVM
    {
        public CharacterCertificateRequestVM CharacterCertificateInfo { get; set; }

        public TaxEntityViewModel TaxEntity { get; set; }

        public string ApprovalNumber { get; set; }

        public string FileRefNumber { get; set; }

        public DateTime RequestDate { get; set; }

        public DateTime ApprovalDate { get; set; }

        public string ViewName { get; set; }

        public PSSRequestStatus RequestStatus { get; set; }

        public CBSUserVM CbsUser { get; set; }
    }
}