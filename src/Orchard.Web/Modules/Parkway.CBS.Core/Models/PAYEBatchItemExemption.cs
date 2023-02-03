using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Models
{
    public class PAYEBatchItemExemption : CBSBaseModel
    {
        public virtual long Id { get; set; }

        public virtual PAYEExemptionType PAYEExemptionType { get; set; }

        public virtual PAYEBatchItems PAYEBatchItems { get; set; }

        public virtual PAYEBatchRecord PAYEBatchRecord { get; set; }

        public virtual decimal Amount { get; set; }
    }
}