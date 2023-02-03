using System.Collections.Generic;

namespace Parkway.CBS.Police.Admin.VM
{
    public class AccountWalletPaymentApprovalRequestVM
    {
        public string From { get; set; }

        public string End { get; set; }

        public string SourceAccount { get; set; }

        public string PaymentId { get; set; }

        public dynamic Pager { get; set; }

        public int TotalAccountWalletPaymentApprovalRecord { get; set; }

        public List<Core.HelperModels.AccountWalletPaymentApprovalReportVM> AccountWalletPaymentApprovalReports { get; set; }
    }
}