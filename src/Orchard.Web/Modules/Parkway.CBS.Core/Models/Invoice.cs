using Orchard.Users.Models;
using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;

namespace Parkway.CBS.Core.Models
{
    public class Invoice : CBSBaseModel
    {

        public virtual Int64 Id { get; set; }

        /// <summary>
        /// string ref in InvoiceCreatedEventHandler
        /// </summary>
        public virtual string InvoiceNumber { get; set; }

        public virtual MDA Mda { get; set; }

        public virtual RevenueHead RevenueHead { get; set; }

        public virtual TaxEntity TaxPayer { get; set; }

        public virtual string InvoiceURL { get; set; }

        /// <summary>
        /// Use InvoiceStatus enum
        /// <see cref="InvoiceStatus"/>
        /// </summary>
        public virtual int Status { get; set; }

        public virtual decimal Amount { get; set; }

        public virtual DateTime DueDate { get; set; }

        public virtual DateTime? PaymentDate { get; set; }

        public virtual TaxEntityCategory TaxPayerCategory { get; set; }

        /// <summary>
        /// this is the unique value cashflow uses to identify if an invoice has been generated before
        /// </summary>
        public virtual string CashflowInvoiceIdentifier { get; set; }

        public virtual IEnumerable<TransactionLog> Payments { get; set; }

        public virtual string InvoiceModel { get; set; }

        /// <summary>
        /// Expert system that generated this invoice
        /// </summary>
        public virtual ExpertSystemSettings ExpertSystemSettings { get; set; }


        /// <summary>
        /// External ref number
        /// </summary>
        public virtual string ExternalRefNumber { get; set; }


        public virtual InvoiceAmountDueSummary InvoiceAmountDueSummary { get; set; }

        public virtual UserPartRecord GeneratedByAdminUser { get; set; }

        public virtual string InvoiceDescription { get; set; }

        public virtual string CallBackURL { get; set; }

        public virtual APIRequest APIRequest { get; set; }

        /// <summary>
        /// <see cref="Models.Enums.InvoiceType"/>
        /// </summary>
        public virtual int InvoiceType { get; set; }

        /// <summary>
        /// This field would hold the invoice type Id, the invoice type Id tells us what the Id of the invoice type is on the 
        /// table that has the type Id
        /// <para>For example, direct assessment invoice type, the invoice type Id would house the Id of the direct assessment batch record on that
        /// direct assessment batch record table</para>
        /// </summary>
        public virtual Int64 InvoiceTypeId { get; set; }


        //////////////////////////////////////////////////////////////////////////////////////////////////////////

        public virtual string InvoiceTitle { get; set; }

        public virtual IEnumerable<InvoiceItems> InvoiceItems { get; set; }

        /// <summary>
        /// hold prehistoric nagis invoice numbers
        /// </summary>
        public virtual string NAGISInvoiceNumber { get; set; }


        /// <summary>
        /// List of third party payment providers
        /// </summary>
        public virtual IEnumerable<PaymentReference> PaymentProviderPaymentReferences { get; set; }


        public InvoiceType GetInvoiceType()
        {
            return ((InvoiceType) this.InvoiceType);
        }

        public virtual int Quantity { get; set; }

        public virtual Boolean IsCancel { get; set; }

        public virtual DateTime? CancelDate { get; set; }

        public virtual UserPartRecord CancelBy { get; set; }
    }
}