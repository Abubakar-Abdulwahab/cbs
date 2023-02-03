using Orchard.Users.Models;
using Parkway.CBS.Core.Models.Enums;
using System;

namespace Parkway.CBS.Core.Models
{
    public class TransactionLog : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual Invoice Invoice { get; set; }

        public virtual InvoiceItems InvoiceItem { get; set; }

        public virtual decimal AmountPaid { get; set; }

        public virtual DateTime PaymentDate { get; set; }

        /// <summary>
        /// Payment ref here is the ref sent to the payment processor
        /// </summary>
        public virtual string PaymentReference { get; set; }

        /// <summary>
        /// Unique value constraint for payment reference
        /// Combination of the paymentReference, channel, mda and revenue head
        /// So a payment reference for a certain channel, mda and revenue head can
        /// only occur once
        /// </summary>
        public virtual string CompositeUniquePaymentReference { get; set; }

        public virtual PaymentStatus Status { get; set; }


        public virtual TaxEntity TaxEntity { get; set; }


        public virtual TaxEntityCategory TaxEntityCategory { get; set; }


        /// <summary>
        /// corres enum type <see cref="PaymentChannel"/>
        /// </summary>
        public virtual int Channel { get; set; }

        /// <summary>
        /// <see cref="Enums.PaymentProvider"/>
        /// </summary>
        public virtual int PaymentProvider { get; set; }


        public virtual string InvoiceNumber { get; set; }

        /// <summary>
        /// Pay Direct Web object dump maps -> PayDirectWebServerResponse
        /// </summary>
        public virtual string RequestDump { get; set; }


        public virtual DateTime? TransactionDate { get; set; }

        public virtual string PayerName { get; set; }

        public virtual string PayerEmail { get; set; }

        public virtual string Bank { get; set; }

        public virtual string BankCode { get; set; }

        public virtual string BankBranch { get; set; }

        public virtual string BankChannel { get; set; }

        public virtual string AgencyCode { get; set; }

        public virtual string RevenueHeadCode { get; set; }

        public virtual string ServiceType { get; set; }

        public virtual UserPartRecord AdminUser { get; set; }

        public virtual bool UpdatedByAdmin { get; set; }

        public virtual RevenueHead RevenueHead { get; set; }

        public virtual MDA MDA { get; set; }

        public virtual Receipt Receipt { get; set; }

        public virtual string ReceiptNumber { get; set; }

        public virtual string ThirdPartyReceiptNumber { get; set; }

        public virtual string SlipNumber { get; set; }

        public virtual string TellerName { get; set; }

        public virtual string PayerPhoneNumber { get; set; }

        public virtual string PayerAddress { get; set; }

        public virtual string ItemName { get; set; }

        public virtual string ItemCode { get; set; }

        public virtual string PaymentMethod { get; set; }

        public virtual int PaymentMethodId { get; set; }

        public virtual string PaymentLogId { get; set; }

        /// <summary>
        /// RetrievalReferenceNumber for pay direct web
        /// </summary>
        public virtual string RetrievalReferenceNumber { get; set; }

        /// <summary>
        /// PaymentType
        /// <see cref="PaymentType"/>
        /// </summary>
        public virtual Int32 TypeID { get; set; }

        /// <summary>
        /// For debit, reversal in the case of Paydirect, this would contain the originalPaymentLogId
        /// </summary>
        public virtual string OriginalPaymentLogID { get; set; }

        /// <summary>
        /// For debit, reversal in the case of Paydirect, this would contain the originalPaymentReference
        /// </summary>
        public virtual string OriginalPaymentReference { get; set; }

        /// <summary>
        /// Total amount paid, iff a fee applies
        /// </summary>
        public virtual decimal TotalAmountPaid { get; set; }

        /// <summary>
        /// indicates whether this transaction has been settled
        /// </summary>
        public virtual bool Settled { get; set; }

        /// <summary>
        /// fee
        /// </summary>
        public virtual decimal Fee { get; set; }


        /// <summary>
        /// indicates that this transaction has been reversed
        /// </summary>
        public virtual bool Reversed { get; set; }


        /// <summary>
        /// this prop holds the amount that was deducted from the customer
        /// amount paid.
        /// </summary>
        public virtual decimal SettlementFeeDeduction { get; set; }

        /// <summary>
        /// This is the amount we want to use for settlement purposes
        /// </summary>
        public virtual decimal SettlementAmount { get; set; }

        public virtual DateTime? SettlementDate { get; set; }

        public virtual Int64? SettlmentBatchIdentifier { get; set; }

        public PaymentType GetPaymentType()
        {
            return ((PaymentType)this.TypeID);
        }
    }    
}