using System;

namespace Parkway.CBS.Core.Models
{
    public class FormControlRevenueHeadValue : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual FormControlRevenueHead FormControlRevenueHead { get; set; }

        public virtual Invoice Invoice { get; set; }

        public virtual InvoiceItems InvoiceItem { get; set; }

        public virtual string Value { get; set; }
    }
}