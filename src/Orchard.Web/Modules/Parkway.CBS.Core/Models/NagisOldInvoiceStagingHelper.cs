using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Models
{
    public class NagisOldInvoiceStagingHelper : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual Int64 TaxEntity_Id { get; set; }

        public virtual string CustomerId { get; set; }

        public virtual Int32 NagisDataBatch_Id { get; set; }

        public virtual Int64 SourceId { get; set; }

    }
}