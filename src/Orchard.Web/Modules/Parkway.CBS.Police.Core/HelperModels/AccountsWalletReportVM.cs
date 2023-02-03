using System;

namespace Parkway.CBS.Police.Core.HelperModels
{
    public class AccountsWalletReportVM
    {
        public string FeePartyBank { get;  set; }

        public string FeePartyAccountName { get;  set; }

        public  string FeePartyAccountNumber { get; set; }

        public int AccountWalletId { get; set; }

        public int FlowDefinitionId { get; set; }

        public string AccountNumber { get; set; }

        public string AccountName { get; set; }

        public string Bank { get; set; }
        
        public DateTime UpdatedAtUtc { get; set; }
    }
}