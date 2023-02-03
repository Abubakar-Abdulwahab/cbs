using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.HelperModels
{
    public class DeploymentAllowancePaymentReportVM
    {
        public string From { get; set; }

        public string End { get; set; }

        public string SourceAccount { get; set; }

        public string PaymentRef { get; set; }

        public int Status { get; set; }

        public dynamic Pager { get; set; }

        public int TotalDeploymentAllowancePaymentReportRecord { get; set; }

        public decimal TotalDeploymentAllowancePaymentReportAmount { get; set; }

        public List<DeploymentAllowancePaymentReportItemVM> DeploymentAllowancePaymentReportItems { get; set; }

        public string TenantName { get; set; }

        public string LogoURL { get; set; }

        public int CommandTypeId { get; set; }

        public IEnumerable<CommandTypeVM> CommandTypes { get; set; }
    }
}