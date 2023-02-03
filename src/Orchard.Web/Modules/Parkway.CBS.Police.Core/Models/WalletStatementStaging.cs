using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Models
{
    public class WalletStatementStaging : CBSBaseModel
    {
        public virtual long Id { get; set; }

        public virtual DateTime TransactionDate { get; set; }

        public virtual DateTime ValueDate { get; set; }

        public virtual string Narration { get; set; }

        public virtual int TransactionTypeId { get; set; }

        public virtual decimal Amount { get; set; }

        public virtual string TransactionReference { get; set; }

        public virtual int WalletId { get; set; }

        /// <summary>
        /// <see cref="Enums.WalletIdentifierType"/>
        /// </summary>
        public virtual int WalletIdentifierType { get; set; }

        /// <summary>
        /// This represents the reference per batch, created to identify records that
        /// should be moved to the main table during the request process after successfully
        /// retrieving statements for a batch
        /// </summary>
        public virtual string Reference { get; set; }
    }
}