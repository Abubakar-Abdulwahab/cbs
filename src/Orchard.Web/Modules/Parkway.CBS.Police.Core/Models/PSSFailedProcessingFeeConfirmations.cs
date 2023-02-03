using System;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    public class PSSFailedProcessingFeeConfirmations : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual string Message { get; set; }

        public virtual string InvoiceNumber { get; set; }

        public virtual Int64 RequestId { get; set; }

        public virtual bool NeedsAction { get; set; }
    }
}