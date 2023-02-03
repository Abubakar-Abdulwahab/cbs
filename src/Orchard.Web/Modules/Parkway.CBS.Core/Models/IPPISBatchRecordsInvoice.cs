using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Models
{
    public class IPPISBatchRecordsInvoice : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual IPPISBatch IPPISBatch { get; set; }

        public virtual TaxEntity TaxEntity { get; set; }

        public virtual TaxEntityCategory TaxEntityCategory { get; set; }

        public virtual IPPISTaxPayerSummary IPPISTaxPayerSummary { get; set; }

        public virtual string InvoiceNumber { get; set; }

        public virtual string InvoiceModel { get; set; }

        public virtual decimal InvoiceAmount { get; set; }

        public virtual string CashflowInvoiceIdentifier { get; set; }

        public virtual Int64 PrimaryContactId { get; set; }

        public virtual Int64 CashflowCustomerId { get; set; }

        public virtual bool ErrorOccurred { get; set; }

        public virtual string ErrorCode { get; set; }

        public virtual string ErrorMessage { get; set; }

        public virtual DateTime DueDate { get; set; }

        public virtual string InvoiceDescription { get; set; }
    }
}