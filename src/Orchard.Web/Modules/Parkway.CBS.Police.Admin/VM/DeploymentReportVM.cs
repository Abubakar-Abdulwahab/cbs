using System.Collections.Generic;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Admin.VM
{
    public class DeploymentReportVM
    {
        public string From { get; set; }

        public string End { get; set; }

        public string CustomerName { get; set; }

        public string RequestRef { get; set; }

        public string IPPISNo { get; set; }
        
        public string APNumber { get; set; }

        public int State { get; set; }

        public int LGA { get; set; }

        public int Rank { get; set; }

        public string OfficerName { get; set; }

        public List<StateModel> StateLGAs { get; set; }

        public List<LGA> ListLGAs { get; set; }

        public dynamic Pager { get; set; }

        public List<CommandVM> Commands { get; set; }

        public string SelectedCommand { get; set; }

        public int CommandId { get; set; }

        public List<PoliceRankingVM> Ranks { get; set; }

        public IEnumerable<PoliceOfficerDeploymentVM> ReportRecords { get; set; }

        public int TotalNumberOfDeployments { get; set; }

        public int TotalNumberOfActiveDeployments { get; set; }

        public int TotalNumberOfOfficersInActiveDeployments { get; set; }

        public string InvoiceNumber { get; set; }

        /// <summary>
        /// Logo URL
        /// </summary>
        public string LogoURL { get; set; }

        /// <summary>
        /// Tenant name
        /// </summary>
        public string TenantName { get; set; }

        public string SelectedCommandName { get; set; }

        public string SelectedCommandCode { get; set; }

    }
}