using System;

namespace Parkway.CBS.Core.Models
{
    public class InvoiceItems : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual MDA Mda { get; set; }

        public virtual RevenueHead RevenueHead { get; set; }

        public virtual decimal UnitAmount { get; set; }

        public virtual Invoice Invoice { get; set; }

        public virtual TaxEntity TaxEntity { get; set; }

        public virtual TaxEntityCategory TaxEntityCategory { get; set; }

        /// <summary>
        /// string ref in InvoiceCreatedEventHandler
        /// </summary>
        public virtual string InvoiceNumber { get; set; }

        public virtual int Quantity { get; set; }

        /// <summary>
        /// unique invoicing identifier on cashflow
        /// </summary>
        public virtual string InvoicingUniqueIdentifier { get; set; }


        /// <summary>
        /// Computed column of unit amount * quantity
        /// </summary>
        public virtual decimal TotalAmount { get; set; }

    }
}