using CBSPay.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBSPay.Core.Models
{
    /// <summary>
    /// contains payment details from the different payment channels after payment has been made
    /// </summary>
    public class PaymentHistory : BaseEntity<long>
    {
        public virtual string ReferenceNumber { get; set; }
        public virtual string TaxPayerMobileNumber { get; set; }
        public virtual string TaxPayerName { get; set; }
        public virtual string TaxPayerTIN { get; set; }
        public virtual string TaxPayerRIN { get; set; }
        public virtual long TaxPayerID { get; set; }
        public virtual long TaxPayerTypeID { get; set; }
        public virtual string PaymentChannel { get; set; }
        public virtual string UpdatedPaymentChannel { get; set; }
        /// <summary>
        /// Mandatory - Amount paid by the customer in Major Denomination. 
        /// This must be verified before giving value to the Customer. 
        /// For reversal payment notification, the amount is negative
        /// </summary>
        public virtual decimal AmountPaid { get; set; }
        public virtual decimal TotalAmountPaid { get; set; }
        public virtual decimal ReferenceAmount { get; set; }
        public virtual string PaymentIdentifier { get; set; }
        public virtual bool IsCustomerDeposit { get; set; }

        //pay direct params
        /// <summary>
        /// To know if the notification has been previously sent
        /// </summary>
        public virtual bool IsRepeated { get; set; }
        /// <summary>
        /// Mandatory - Unique integer ID for the payment
        /// </summary>
        public virtual int PaymentLogId { get; set; }
        /// <summary>
        /// Mandatory - Unique reference for the payment as issued to the customer at the point of payment. 
        /// This value is unique for all transactions.
        /// </summary>
        public virtual string PaymentReference { get; set; }
        /// <summary>
        /// Mandatory - Unique ID for the paying customer or specific order. 
        /// Typically provided to the paying customer by the merchant
        /// </summary>
        public virtual string CustReference { get; set; }
        public virtual string AlternateCustReference { get; set; }
        /// <summary>
        /// Mandatory - Method by which customer made payment. 
        /// </summary>
        public virtual string PaymentMethod { get; set; }
        /// <summary>
        /// Mandatory - Name of channel used for transaction. 
        /// </summary>
        public virtual string ChannelName { get; set; }
        /// <summary>
        /// Location payment was made
        /// </summary>
        public virtual string Location { get; set; }
        /// <summary>
        /// mandatory - This specifies if the notification is a reversal payment notification
        /// </summary>
        public virtual bool IsReversal { get; set; }
        /// <summary>
        /// Mandatory - Date payment was made in format MM/DD/YYYY hh:mm:ss
        /// </summary>
        public virtual DateTime? PaymentDate { get; set; }
        /// <summary>
        /// mandatory - Date payment would be settled into Merchant account in format MM/DD/YYYY hh:mm:ss
        /// </summary>
        public virtual DateTime? SettlementDate { get; set; }
        /// <summary>
        /// Mandatory - Unique ID given to the merchant
        /// </summary>
        public virtual string InstitutionId { get; set; }
        /// <summary>
        /// Mandatory - Merchant’s configured name within the Bill payment system
        /// </summary>
        public virtual string InstitutionName { get; set; }
        /// <summary>
        /// Bank Branch where the payment was made, if applicable
        /// </summary>
        public virtual string BranchName { get; set; }
        /// <summary>
        /// Bank where the payment was made in case of a cash payment or 
        /// bank whose card was used to pay in the case of a card based payment
        /// </summary>
        public virtual string BankName { get; set; }
        public virtual string FeeName { get; set; }
        /// <summary>
        /// Mandatory - Receipt Number issued to customer
        /// </summary>
        public virtual string ReceiptNo { get; set; }
        public virtual int PaymentCurrency { get; set; }
        /// <summary>
        /// Used in Payment Reversal Notification to indicate the Unique Integer ID for the transaction which needs to be reversed
        /// </summary>
        public virtual int OriginalPaymentLogId { get; set; }
        /// <summary>
        /// Used in Payment Reversal Notification to indicate the Payment reference for the transaction which needs to be reversed
        /// </summary>
        public virtual string OriginalPaymentReference { get; set; }
        /// <summary>
        /// The teller name
        /// </summary>
        public virtual string Teller { get; set; }


        public virtual decimal SettlementAmount { get; set; }

        public virtual int SettlementMethodID { get; set; }

        public virtual string SettlementMethodName { get; set; }

        public virtual int SettlementStatusID { get; set; }

        public virtual string SettlementStatusName { get; set; }

       
        public virtual string SettlementNotes { get; set; }
        /// <summary>
        /// AssessmentID (e.g 10009) for assessment and ServiceBillID (e.g 10007) for service bill
        /// </summary>
        public virtual long ReferenceID { get; set; }
        public virtual bool IsSyncedWithEIRS { get; set; }
        public virtual int Trials { get; set; }
        public virtual IEnumerable<PaymentHistoryItem> PaymentItemsHistory { get; set; }
        //public virtual ISet<PaymentHistoryItem> PaymentItemsHistory { get; set; }
        //public virtual IList<PaymentHistoryItem> PaymentItemsHistory { get; set; }
        //public virtual ICollection<PaymentHistoryItem> PaymentItemsHistory { get; set; }
        //NO RIN Capture objects
        public virtual string TaxPayerType { get; set; }
        public virtual string Email { get; set; }
        public virtual string EconomicActivity { get; set; }
        public virtual string Address { get; set; }
        public virtual string RevenueStream { get; set; }
        public virtual string RevenueSubStream { get; set; }
        public virtual string OtherInformation { get; set; }
    }
}
