using Parkway.CBS.Core.HelperModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Core.VM
{
    public class InvestigationReportRequestDetailsVM
    {
        public string Reason { get; set; }

        public string StateName { get; set; }

        public string LGAName { get; set; }

        public string CommandName { get; set; }

        public string CommandAddress { get; set; }

        public TaxEntityViewModel TaxEntity { get; set; }

        public string Comment { get; set; }

        public int ServiceTypeId { get; set; }

        public Int64 RequestId { get; set; }

        public string ViewName { get; set; }

        public int ApprovalStatus { get; set; }

        public int ApproverId { get; set; }

        public string CallBackURL { get; set; }

        public string ServiceName { get; set; }

    }
}