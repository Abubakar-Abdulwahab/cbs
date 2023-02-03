using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Models
{
    public class ReferenceDataTypeOfTaxPaidMapping : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual ReferenceDataRecords ReferenceDataRecords { get; set; }

        public int SerialNumberId { get; internal set; }

        public int ReferenceDataTypeOfTaxPaid { get; internal set; }

        public virtual ReferenceDataBatch ReferenceDataBatch { get; set; }
    }
}