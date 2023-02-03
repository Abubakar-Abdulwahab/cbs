using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Models
{
    /// <summary>
    /// Contains Reference to IPPISBatch and ReferenceDataBatch
    /// </summary>
    public class GeneralBatchReference : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual string BatchRef { get; set; }

        public virtual string AdapterClassName { get; set; }
    }
}