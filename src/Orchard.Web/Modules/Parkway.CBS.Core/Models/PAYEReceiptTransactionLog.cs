using System;

namespace Parkway.CBS.Core.Models
{
    public class PAYEReceiptTransactionLog : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual PAYEReceipt PAYEReceipt { get; set; }

        public virtual Receipt Receipt { get; set; }

        public virtual TransactionLog TransactionLog { get; set; }
    }
}