using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Models
{
    public class NagisOldInvoices : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual TaxEntity TaxEntity { get; set; }

        public virtual NagisDataBatch NagisDataBatch { get; set; }

        public virtual TaxEntityCategory TaxEntityCategory { get; set; }

        public virtual RevenueHead RevenueHead { get; set; }

        public virtual NagisOldInvoiceSummary NagisOldInvoiceSummary { get; set; }

        public virtual string CustomerName { get; set; }

        public virtual string Address { get; set; }

        public virtual string PhoneNumber { get; set; }

        public virtual string CustomerId { get; set; }

        public virtual decimal Amount { get; set; }

        public virtual string TIN { get; set; }

        public virtual string NagisInvoiceNumber { get; set; }

        public virtual DateTime NagisInvoiceCreationDate { get; set; }

        public virtual string ExternalRefId { get; set; }

        public virtual string InvoiceDescription { get; set; }

        public virtual decimal AmountDue { get; set; }

        public virtual int Quantity { get; set; }

        public virtual int Status { get; set; }

        public virtual int GroupId { get; set; }

        // This is use to determine if the Payer details will be created in TaxEntity table
        // or updated if already exist
        public virtual int OperationType { get; set; }

        public virtual ReferenceDataOperationType GetOperationType()
        {
            return (ReferenceDataOperationType)this.OperationType;
        }


    }
}