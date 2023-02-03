using System;
using System.Collections.Generic;

namespace Parkway.CBS.Core.Models
{
    public class Receipt : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual Invoice Invoice { get; set; }

        public virtual string ReceiptNumber { get; set; }

        public IEnumerable<TransactionLog> TransactionLogs { get; set; }
    }
}