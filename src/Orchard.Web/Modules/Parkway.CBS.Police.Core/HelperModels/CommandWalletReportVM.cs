using Parkway.CBS.Core.Models.Enums;

namespace Parkway.CBS.Police.Core.HelperModels
{
    public class CommandWalletReportVM
    {
        public string AccountNumber { get; set; }

        public string BankCode { get; set; }

        public string CommandCode { get; set; }

        public string CommandName { get; set; }

        public string Balance { get; set; }

        public int SettlementAccountType { get; set; }

        public string SettlementAccountTypeString => ((Models.Enums.SettlementAccountType)SettlementAccountType).GetDescription();
    }
}