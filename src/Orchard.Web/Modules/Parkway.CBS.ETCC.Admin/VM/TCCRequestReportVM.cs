using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models.Enums;
using System.Collections.Generic;

namespace Parkway.CBS.ETCC.Admin.VM
{
    public class TCCRequestReportVM
    {
        public string ApplicationNumber { get; set; }

        public TCCRequestStatus Status { get; set; }

        public string From { get; set; }

        public string End { get; set; }

        public List<TCCRequestVM> Requests { get; set; }

        public dynamic Pager { get; set; }

        public int TotalRequestRecord { get; set; }

        /// <summary>
        /// Logo URL
        /// </summary>
        public string LogoURL { get; set; }

        /// <summary>
        /// Tenant name
        /// </summary>
        public string TenantName { get; set; }

        public string PayerId { get; set; }

        public string ApplicantName { get; set; }
    }
}