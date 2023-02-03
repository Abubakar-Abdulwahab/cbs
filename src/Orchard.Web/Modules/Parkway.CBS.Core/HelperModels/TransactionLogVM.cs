using System;
using Orchard.Users.Models;

namespace Parkway.CBS.Core.HelperModels
{
    public class TransactionLogVM
    {
        public Int64 TransactionLogId { get; set; }

        public decimal AmountPaid { get; set; }

        public DateTime PaymentDate { get; set; }

        /// <summary>
        /// Payment ref here is the ref sent to the payment processor
        /// </summary>
        public string PaymentReference { get; set; }

        public Models.Enums.PaymentStatus Status { get; set; }

        /// <summary>
        /// corres enum type <see cref="PaymentChannel"/>
        /// </summary>
        public int Channel { get; set; }


        /// <summary>
        /// <see cref="Enums.PaymentProvider"/>
        /// </summary>
        public int PaymentProvider { get; set; }


        public string InvoiceNumber { get; set; }

        /// <summary>
        /// Pay Direct Web object dump maps -> PayDirectWebServerResponse
        /// </summary>
        public string RequestDump { get; set; }


        public DateTime? TransactionDate { get; set; }

        public string PayerName { get; set; }

        public string PayerEmail { get; set; }

        public string Bank { get; set; }

        public string BankCode { get; set; }

        public string BankBranch { get; set; }

        public string BankChannel { get; set; }

        public string AgencyCode { get; set; }

        public string RevenueHeadCode { get; set; }

        public string ServiceType { get; set; }

        public UserPartRecord AdminUser { get; set; }

        public bool UpdatedByAdmin { get; set; }

        public string ThirdPartyReceiptNumber { get; set; }

        public string SlipNumber { get; set; }

        public string TellerName { get; set; }

        public string PayerPhoneNumber { get; set; }

        public string PayerAddress { get; set; }

        public string ItemName { get; set; }

        public string ItemCode { get; set; }

        public string PaymentMethod { get; set; }

        public int PaymentMethodId { get; set; }

        public string PaymentLogId { get; set; }

        /// <summary>
        /// RetrievalReferenceNumber for pay direct web
        /// </summary>
        public string RetrievalReferenceNumber { get; set; }

        /// <summary>
        /// PaymentType
        /// <see cref="PaymentType"/>
        /// </summary>
        public Int32 TypeID { get; set; }

        /// <summary>
        /// For debit, reversal in the case of Paydirect, this would contain the originalPaymentLogId
        /// </summary>
        public string OriginalPaymentLogID { get; set; }

        /// <summary>
        /// For debit, reversal in the case of Paydirect, this would contain the originalPaymentReference
        /// </summary>
        public string OriginalPaymentReference { get; set; }

        /// <summary>
        /// Total amount paid, iff a fee applies
        /// </summary>
        public decimal TotalAmountPaid { get; set; }

        /// <summary>
        /// fee
        /// </summary>
        public decimal Fee { get; set; }

        public bool IsReversal { get; set; }

        /// <summary>
        /// this value will hold the agent fee 
        /// that was deducted from the amount the user has paid
        /// </summary>
        public decimal AgentFee { get; set; }

        public bool AllowAgentFeeAddition { get; set; }
        public int RevenueHeadId { get; internal set; }
    }    
}