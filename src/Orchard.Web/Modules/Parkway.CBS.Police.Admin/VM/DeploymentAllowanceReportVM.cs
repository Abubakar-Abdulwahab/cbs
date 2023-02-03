using Parkway.CBS.Core.Models;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Admin.VM
{
    public class DeploymentAllowanceReportVM
    {
        public string From { get; set; }

        public string End { get; set; }

        public string AccountNumber { get; set; }

        public string FileNumber { get; set; }

        public string InvoiceNumber { get; set; }

        public string IPPISNo { get; set; }

        public string APNumber { get; set; }

        public Int64 Rank { get; set; }

        public DeploymentAllowanceStatus Status { get; set; }

        public dynamic Pager { get; set; }

        public List<PoliceRankingVM> Ranks { get; set; }

        public IEnumerable<PoliceOfficerDeploymentAllowanceVM> ReportRecords { get; set; }

        public int TotalNumberOfDeploymentAllowances { get; set; }

        public decimal TotalAmountOfDeploymentAllowances { get; set; }

        /// <summary>
        /// Logo URL
        /// </summary>
        public string LogoURL { get; set; }

        /// <summary>
        /// Tenant name
        /// </summary>
        public string TenantName { get; set; }

        public List<CommandVM> Commands { get; set; }

        public string SelectedCommand { get; set; }

        public int CommandId { get; set; }

        public string SelectedCommandName { get; set; }

        public string SelectedCommandCode { get; set; }

        public int State { get; set; }

        public int LGA { get; set; }

        public List<StateModel> StateLGAs { get; set; }

        public List<LGA> ListLGAs { get; set; }

    }
}