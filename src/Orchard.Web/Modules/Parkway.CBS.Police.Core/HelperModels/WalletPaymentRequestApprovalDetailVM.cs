using System;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.HelperModels
{
    public class WalletPaymentRequestApprovalDetailVM
    {
        public string SourceAccount { get; set; }

        public string PaymentId { get; set; }

        public string SourceAccountNumber { get; set; }

        public int NoOfBeneficiaries { get; set; }

        public DateTime DateInitiated { get; set; }

        public decimal TotalAmountWalletPaymentApprovalRequestRecord { get; set; }

        public List<AccountWalletPaymentRequestItemDetailVM> WalletPaymentRequestItemDetails { get; set; }

        public string ApprovalButtonName { get; set; }
    }
}