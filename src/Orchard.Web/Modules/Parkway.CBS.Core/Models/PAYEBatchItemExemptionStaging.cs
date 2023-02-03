using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Models
{
    public class PAYEBatchItemExemptionStaging : CBSBaseModel
    {
        public virtual long Id { get; set; }

        public virtual PAYEExemptionType PAYEExemptionType { get; set; }

        public virtual string PAYEExemptionTypeName { get; set; }

        public virtual PAYEBatchItemsStaging PAYEBatchItemsStaging { get; set; }

        public virtual PAYEBatchRecordStaging PAYEBatchRecordStaging { get; set; }

        public virtual decimal Amount { get; set; }

        public virtual string AmountStringValue { get; set; }

        /// <summary>
        /// Unique identifier for each batch record items
        /// </summary>
        public virtual int SerialNumber { get; set; }
    }
}