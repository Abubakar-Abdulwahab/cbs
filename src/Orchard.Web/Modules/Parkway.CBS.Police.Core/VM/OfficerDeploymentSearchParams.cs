using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Core.VM
{
    public class OfficerDeploymentSearchParams
    {
        public int Skip { get; set; }

        public int Take { get; set; }

        public int AdminUserId { get; set; }

        public bool DontPageData { get; set; }

        public int CommandId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string SelectedCommand { get; set; }

        public int State { get; set; }

        public int LGA { get; set; }

        public string InvoiceNumber { get; set; }

        public string CustomerName { get; set; }

        public string Address { get; set; }

        public string APNumber { get; set; }

        public string RequestRef { get; set; }

        public string IPPISNo { get; set; }

        public int Rank { get; set; }

        public string OfficerName { get; set; }

        public int ApprovalAccessRoleUserId { get; set; }
    }
}