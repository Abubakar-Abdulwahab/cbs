using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Core.VM
{
    public class RequestsReportSearchParams
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int Take { get; set; }

        public int Skip { get; set; }

        public RequestOptions RequestOptions { get; set; }

        public int AdminUserId { get; set; }

        public string SelectedServiceId { get; set; }

        public int IntValueSelectedServiceId { get; set; }

        public bool DontPageData { get; set; }

        public bool IsBranchAdmin { get; set; }

        public Int64 TaxEntityId { get; set; }

        public Int64 CBSUserId { get; set; }

        public string SelectedRevenueHead { get; set; }

        public int CommandId { get; set; }

        public string SelectedCommand { get; set; }

        public string BranchName { get; set; }

        public int Branch { get; set; }

        public string OrderByColumnName { get; set; }

        public int State { get; set; }

        public int LGA { get; set; }

        public int ApprovalAccessRoleUserId { get; set; }

        public bool CheckWorkFlowLogActiveStatus { get; set; }

        public int SelectedRequestPhase { get; set; }
    }
}