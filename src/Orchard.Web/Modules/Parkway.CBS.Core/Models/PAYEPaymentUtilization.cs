using System;

namespace Parkway.CBS.Core.Models
{
    public class PAYEPaymentUtilization : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual PAYEBatchRecord PAYEBatchRecord { get; set; }

        public virtual PAYEReceipt PAYEReceipt { get; set; }

        public virtual decimal UtilizedAmount { get; set; }
    }
}