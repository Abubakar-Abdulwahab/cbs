using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.Models.Enums;
using System;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.VM
{
    public class EscortDetailsVM
    {
        public EscortRequestVM EscortInfo { get; set; }

        public TaxEntityViewModel TaxEntity { get; set; }

        public HeaderObj HeaderObj { get; set; }

        public List<PSSRequestStatusLogVM> RequestStatusLog { get; set; }

        public string ViewName { get; set; }

        public DateTime RequestDate { get; set; }

        public DateTime ApprovalDate { get; set; }

        public int RequestStatus { get; set; }

        public CBSUserVM CbsUser { get; set; }
    }
}