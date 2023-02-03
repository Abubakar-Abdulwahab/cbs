using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Admin.VM
{
    public class AccountWalletPaymentReportVM
    {
        public string From { get; set; }

        public string End { get; set; }

        public string SourceAccount { get; set; }

        public string BeneficiaryAccountNumber { get; set; }

        public int ExpenditureHeadId { get; set; }

        public string PaymentId { get; set; }

        public int Status { get; set; }

        public dynamic Pager { get; set; }

        public int TotalAccountWalletPaymentReportRecord { get; set; }

        public decimal TotalAccountWalletPaymentReportAmount { get; set; }

        public List<ExpenditureHeadVM> ExpenditureHeads { get; set; }

        public List<Core.HelperModels.AccountWalletPaymentReportVM> AccountWalletPaymentReports { get; set; }

        public string TenantName { get; set; }

        public string LogoURL { get; set; }
    }
}