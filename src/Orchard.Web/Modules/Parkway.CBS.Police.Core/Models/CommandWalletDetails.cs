using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    public class CommandWalletDetails : CBSModel
    {
        public virtual string AccountNumber { get; set; }

        public virtual string BankCode { get; set; }

        public virtual bool IsActive { get; set; }

        public virtual Command Command { get; set; }

        public virtual Bank Bank { get; set; }

        /// <summary>
        /// <see cref="Enums.SettlementAccountType"/>
        /// </summary>
        public virtual int SettlementAccountType { get; set; }

    }
}