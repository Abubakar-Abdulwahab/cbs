using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models.Enums;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Admin.VM
{
    public class CommandStatementReportVM
    {
        public string Balance { get; set; }

        public IEnumerable<CommandWalletStatementVM> CommandWalletStatements { get; set; }

        public string End { get; set; }

        public string From { get; set; }

        public string LogoURL { get; set; }

        public dynamic Pager { get; set; }

        public string TenantName { get; set; }

        public int TotalCommandWalletStatementRecord { get; set; }

        public string TransactionReference { get; set; }

        public TransactionType TransactionType { get; set; }

        public string ValueDate { get; set; }

        public CommandWalletReportVM WalletReportVM { get; set; }
    }
}