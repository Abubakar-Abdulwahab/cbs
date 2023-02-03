using Parkway.CBS.Police.Core.Models.Enums;
using System;

namespace Parkway.CBS.Police.Core.HelperModels
{
    public class CommandWalletStatementVM
    {
        public decimal Amount { get; set; }

        public decimal Balance { get; set; }

        public string FromAccountName { get; set; }

        public string FromBankName { get; set; }

        public long Id { get; set; }

        public string Narration { get; set; }

        public string ToAccountName { get; set; }

        public string ToBankName { get; set; }

        public DateTime TransactionDate { get; set; }

        public string TransactionReference { get; set; }

        public PSSCommandWalletStatementStatus TransactionStatus { get; set; }

        public int TransactionTypeId { get; set; }

        public DateTime ValueDate { get; set; }
    }
}