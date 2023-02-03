using System;

namespace Parkway.CBS.Core.Models
{
    public class PAYEBatchItemsStagingPivot : CBSBaseModel
    {
        public virtual long Id { get; set; }

        public virtual long PayeBatchItemsStagingId { get; set; }

        public virtual long PayeBatchItemsId { get; set; }

        public virtual long PayeBatchRecordId { get; set; }

    }
}