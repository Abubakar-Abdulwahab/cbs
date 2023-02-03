using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CBSPay.Core.ViewModels
{
    /// <summary>
    /// captures the likely request parameters to be expected from paydirect
    /// </summary>
    public class PaymentNotificationRequest
    {
        /// <summary>
        /// Mandatory - Third party platform service url
        /// </summary>
        public string ServiceUrl { get; set; }
        public string ServiceUsername { get; set; }
        public string ServicePassword { get; set; }        
        /// <summary>
        /// Container element for a collection of payments
        /// </summary>
        public List<Payment> Payments { get; set; }
    } 

    public class CustomerInformationRequest
    {
        /// <summary>
        /// Mandatory - Third party platform service url
        /// </summary>
        public string ServiceUrl { get; set; }
        /// <summary>
        /// Username to access Service at Merchant’s end
        /// </summary>
        public string ServiceUsername { get; set; }
        /// <summary>
        /// Password to access Service at Merchant’s end
        /// </summary>
        public string ServicePassword { get; set; }
        /// <summary>
        /// Mandatory - Unique ID given by Interswitch to the client using the bill payment solution
        /// </summary>
        public string MerchantReference { get; set; }
        /// <summary>
        /// Mandatory - Unique ID for the paying customer or specific order. Typically provided to the paying customer by the merchant
        /// </summary>
        public string CustReference { get; set; }
        /// <summary>
        /// This is an agreed code indicating the item that the customer has indicated he wants to pay for.
        /// </summary>
        public string PaymentItemCode { get; set; }
        public string ThirdPartyCode { get; set; }
        /// <summary>
        /// Agreed code used to group items customers can pay for
        /// </summary>
        public string PaymentItemCategoryCode { get; set; }
        /// <summary>
        /// Amount to be paid, 0 if customer is allowed to pay any amount
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// Unique identifier for Terminal
        /// </summary>
        public string TerminalId { get; set; }
    }

    /// <summary>
    /// Container element for a single payment
    /// </summary>
    public class Payment
    {
        /// <summary>
        /// To know if the notification has been previously sent
        /// </summary>
        public string IsRepeated { get; set; }
        /// <summary>
        /// Used internally by Interswitch systems - Mandatory
        /// </summary>
        public string ProductGroupCode { get; set; }
        /// <summary>
        /// Mandatory - Unique integer ID for the payment
        /// </summary>
        public string PaymentLogId { get; set; }
        /// <summary>
        /// Mandatory - Status of Payment. Note that third party systems would only get notification for successful payments, so this defaults to 0
        /// </summary>
        public int PaymentStatus { get; set; }
        /// <summary>
        /// Acknowledgement returned by Merchant to indicate if payment was received or not 0=Received/Duplicate Payment 1= Rejected by System
        /// </summary>
        public int Status { get; set; }
        public string StatusMessage { get; set; }
        /// <summary>
        /// Mandatory - Unique reference for the payment as issued to the customer at the point of payment. This value is unique for all transactions.
        /// </summary>
        public string PaymentReference { get; set; }
        /// <summary>
        /// Mandatory - Unique ID for the paying customer or specific order. Typically provided to the paying customer by the merchant
        /// </summary>
        public string CustReference { get; set; }
        public string AlternateCustReference { get; set; }
        /// <summary>
        /// Mandatory - Amount paid by the customer in Major Denomination. This must be verified before giving value to the Customer. For reversal payment notification, the amount is negative
        /// </summary>
        public decimal Amount { get; set; }
        // i may change to enum type
        /// <summary>
        /// Mandatory - Method by which customer made payment. See list of acceptable values in appendix
        /// </summary>
        public string PaymentMethod { get; set; }
        /// <summary>
        /// Mandatory - Name of channel used for transaction. See acceptable values in appendix
        /// </summary>
        public string ChannelName { get; set; }
        /// <summary>
        /// Location payment was made
        /// </summary>
        public string Location { get; set; }
        /// <summary>
        /// mandatory - This specifies if the notification is a reversal payment notification
        /// </summary>
        public string IsReversal { get; set; }
        /// <summary>
        /// Mandatory - Date payment was made in format MM/DD/YYYY hh:mm:ss
        /// </summary>
        public string PaymentDate { get; set; }
        /// <summary>
        /// mandatory - Date payment would be settled into Merchant account in format MM/DD/YYYY hh:mm:ss
        /// </summary>
        public string SettlementDate { get; set; }
        /// <summary>
        /// Mandatory - Unique ID given to the merchant
        /// </summary>
        public string InstitutionId { get; set; }
        /// <summary>
        /// Mandatory - Merchant’s configured name within the Bill payment system
        /// </summary>
        public string InstitutionName { get; set; }
        /// <summary>
        /// Bank Branch where the payment was made, if applicable
        /// </summary>
        public string BranchName { get; set; }
        /// <summary>
        /// Bank where the payment was made in case of a cash payment or bank whose card was used to pay in the case of a card based payment
        /// </summary>
        public string BankName { get; set; }
        public string FeeName { get; set; }
        public string CustomerName { get; set; }
        public string OtherCustomerInfo { get; set; }
        /// <summary>
        /// Mandatory - Receipt Number issued to customer
        /// </summary>
        public string ReceiptNo { get; set; }
        
        /// <summary>
        /// The account of the Collecting Bank
        /// </summary>
        public string CollectionsAccount { get; set; }
        public string ThirdPartyCode { get; set; }
        /// <summary>
        /// Container element for a collection of payment items
        /// </summary>
        public List<PaymentItem> PaymentItems { get; set; }
        /// <summary>
        /// A code representing the Bank where the payment was made or bank whose card was used to pay in the case of a card based payment
        /// </summary>
        public string BankCode { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public string DepositorName { get; set; }
        /// <summary>
        /// The number on the Deposit Slip used for payments
        /// </summary>
        public string DepositorSlipNumber { get; set; }
        /// <summary>
        /// Mandatory - The Code that identifies the currency in which the payment was made. 566 for naira.
        /// </summary>
        public string PaymentCurrency { get; set; }
        /// <summary>
        /// Used in Payment Reversal Notification to indicate the Unique Integer ID for the transaction which needs to be reversed
        /// </summary>
        public string OriginalPaymentLogId { get; set; }
        /// <summary>
        /// Used in Payment Reversal Notification to indicate the Payment reference for the transaction which needs to be reversed
        /// </summary>
        public string OriginalPaymentReference { get; set; }
        /// <summary>
        /// The teller name
        /// </summary>
        public string Teller { get; set; }        
   }


    /// <summary>
    /// Container element for individual payment item
    /// </summary>
    public class PaymentItem
    {
        /// <summary>
        /// Mandatory - The name of an item that was paid for (There will always be at least one Item)
        /// </summary>
        public string ItemName { get; set; }
        /// <summary>
        /// Mandatory - The code of an item that was paid for (There will always be at least one item)
        /// </summary>
        public string ItemCode { get; set; }
        /// <summary>
        /// Mandatory - The amount that was paid for the item (Will be equal to Amount if only one item was paid for). Note this would be negative for reversal payment notification.
        /// </summary>
        public decimal ItemAmount { get; set; }
        //should be enum
        /// <summary>
        /// This code identifies the bank where the collected funds will be remitted to. See Bank Code in Appendix.
        /// </summary>
        public string LeadBankCode { get; set; }
        //should be enum
        /// <summary>
        /// This is a unique code assigned to the LeadBank (in Nigeria). 
        /// </summary>
        public string LeadBankCbnCode { get; set; }
        //should be enum
        /// <summary>
        /// This is the name of the bank where the collected funds will be remitted to
        /// </summary>
        public string LeadBankName { get; set; }
        public string CategoryCode { get; set; }
        public string CategoryName { get; set; }
        /// <summary>
        /// Payment item count
        /// </summary>
        public int ItemQuantity { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
    }

    
}
