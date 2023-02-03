using System;

namespace Parkway.CBS.Police.Core.VM
{
    public class AccountWalletPaymentApprovalSearchParams
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string SourceAccountName { get; set; }

        public string PaymentId { get; set; }

        public bool DontPageData { get; set; }

        public int Skip { get; set; }

        public int Take { get; set; }
        
        public int UserPartRecordId { get; set; }
        
    }
}