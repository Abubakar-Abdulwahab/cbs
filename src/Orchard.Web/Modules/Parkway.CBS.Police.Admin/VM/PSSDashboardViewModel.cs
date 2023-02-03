using Orchard.Layouts.Framework.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Admin.VM
{
    public class PSSDashboardViewModel
    {
        public IList<CategoryDescriptor> Categories { get; set; }

        public Int64 TotalApprovedRequests { get; set; }

        public Int64 TotalPendingApprovalRequests { get; set; }

        public Int64 TotalRejectedRequests { get; set; }

        public Int64 CurrentUserTotalPendingApprovalRequests { get; set; }

        public Int64 TotalNumberOfOfficersDeployed { get; set; }

        public Int64 TotalNumberOfOfficers { get; set; }

        public string Month { get; set; }

    }
}