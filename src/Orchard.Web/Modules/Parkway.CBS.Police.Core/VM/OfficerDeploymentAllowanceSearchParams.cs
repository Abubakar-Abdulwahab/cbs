using Parkway.CBS.Police.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Core.VM
{
    public class OfficerDeploymentAllowanceSearchParams
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int Take { get; set; }

        public int Skip { get; set; }

        public int AdminUserId { get; set; }

        public Int64 RankId { get; set; }

        public bool DontPageData { get; set; }

        public DeploymentAllowanceStatus RequestStatus { get; set; }

        public string InvoiceNumber { get; set; }

        public string FileNumber { get; set; }

        public string AccountNumber { get; set; }

        public string IPPISNumber { get; set; }

        public string APNumber { get; set; }

        public string SelectedCommand { get; set; }

        public int CommandId { get; set; }

        public int State { get; set; }

        public int LGA { get; set; }

        public int ApprovalAccessRoleUserId { get; set; }
    }
}