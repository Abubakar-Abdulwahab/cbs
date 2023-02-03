using System;

namespace Parkway.CBS.Police.Core.HelperModels
{
    public class AccountWalletPaymentApprovalReportVM
    {
        public int NoOfBeneficiaries { get; set; }

        public long AccountPaymentRequestId { get; set; }

        public string PaymentId { get; set; }

        public string SourceAccount { get; set; }

        public string SourceAccountNumber { get; set; }

        public string TotalAmount { get; set; }

        public DateTime DateInitiated { get; set; }
    }

}