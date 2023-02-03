using System;

namespace Parkway.CBS.Core.Models
{
    public class PAYEBatchItemsStagingValidation : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual PAYEBatchItemsStaging PAYEBatchItemsStaging { get; set; }

        public virtual PAYEBatchRecordStaging PAYEBatchRecordStaging { get; set; }

        public virtual string ErrorMessages { get; set; }
    }
}