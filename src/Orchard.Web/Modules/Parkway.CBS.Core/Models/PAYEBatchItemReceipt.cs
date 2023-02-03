using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Models
{
    public class PAYEBatchItemReceipt : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual string ReceiptNumber { get; set; }

        public virtual PAYEBatchItems PAYEBatchItem { get; set; }
    }
}