using System;

namespace Parkway.CBS.Core.Models
{
    public class PAYEBatchRecordInvoice : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual PAYEBatchRecord PAYEBatchRecord { get; set; }

        public virtual Invoice Invoice { get; set; }
    }
}