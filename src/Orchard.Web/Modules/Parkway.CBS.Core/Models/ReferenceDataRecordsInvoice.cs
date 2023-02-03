using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Models
{
    public class ReferenceDataRecordsInvoice : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual ReferenceDataBatch ReferenceDataBatch { get; set; }

        public virtual TaxEntity TaxEntity { get; set; }

        public virtual TaxEntityCategory TaxEntityCategory { get; set; }

        public virtual Int64 InvoiceUniqueKey { get; set; }

        public virtual string InvoiceNumber { get; set; }

        public virtual string InvoiceModel { get; set; }

        public virtual decimal InvoiceAmount { get; set; }

        public virtual string CashflowInvoiceIdentifier { get; set; }

        public virtual Int64 PrimaryContactId { get; set; }

        public virtual Int64 CashflowCustomerId { get; set; }

        public virtual DateTime DueDate { get; set; }

        public virtual string InvoiceDescription { get; set; }

        public virtual string InvoiceURL { get; set; }
    }
}