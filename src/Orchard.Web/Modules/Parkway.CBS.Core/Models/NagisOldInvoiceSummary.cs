using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Models
{
    public class NagisOldInvoiceSummary : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual TaxEntity TaxEntity { get; set; }

        public virtual NagisDataBatch NagisDataBatch { get; set; }

        public virtual int RevenueHead_Id { get; set; }

        public virtual int MDAId { get; set; }

        public virtual int ExpertSystemId { get; set; }

        public virtual string NagisInvoiceNumber { get; set; }

        public virtual decimal TotalAmount { get; set; }

        public virtual decimal AmountDue { get; set; }

        public virtual int GroupId { get; set; }

        public virtual int StatusId { get; set; }

        public virtual int TaxEntityCategory_Id { get; set; }

        public virtual int NumberOfItems { get; set; }

        public virtual string InvoiceUniqueKey { get; set; }

        public virtual string InvoiceNumber { get; set; }

        public virtual string CashflowInvoiceIdentifier { get; set; }

        public virtual string InvoiceDescription { get; set; }

        public virtual Int64 PrimaryContactId { get; set; }

        public virtual Int64 CashflowCustomerId { get; set; }

        public virtual DateTime DueDate { get; set; }

        public virtual string InvoiceURL { get; set; }

        public virtual List<NagisOldInvoices> InvoiceItems { get; set; }
    }
}