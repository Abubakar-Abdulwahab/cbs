using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Parkway.EbillsPay.Models
{
    public class NotificationRequest
    {
        /// <summary>
        /// Uniquely identifies a transaction
        /// </summary>
        public string SessionID { get; set; }

        /// <summary>
        /// Unique code that identifies the Bank where the payment was made
        /// </summary>
        public string SourceBankCode { get; set; }

        /// <summary>
        /// Identifies the channel where the transaction was initiated. This can be internet banking, ATM etc.
        /// </summary>
        public string ChannelCode { get; set; }

        /// <summary>
        /// Name of the Customer making payment
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// Account number of the Customer making payment
        /// </summary>
        public string CustomerAccountNumber { get; set; }

        /// <summary>
        /// Unique identifier of the biller
        /// </summary>
        public string BillerID { get; set; }

        /// <summary>
        /// Name of the biller
        /// </summary>
        public string BillerName { get; set; }

        /// <summary>
        /// Unique identifier of the product
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// Name of the product
        /// </summary>
        public string ProductName { get; set; }


        /// <summary>
        /// String value. This can be the amount the customer paid (for validation request) 
        /// or the amount that goes to Biller’s account after transaction fee is subtracted 
        /// from the amount paid by customer (for notification request).
        /// </summary>
        [XmlElement(ElementName = "Amount")]
        public string SAmount
        {
            get { return this.Amount.ToString(); }
            set { this.Amount = Convert.ToDecimal(value); }
        }
        //{ get; set; }
        //    get { return this.SAmount.ToString(); }
        //    set { this.Amount = Convert.ToDecimal(value); }
        //}

        //            get { return this.IsRepeated.ToString(); }
        //    set
        //            {
        //                if (string.IsNullOrEmpty(value)) { this.IsRepeated = false; }
        //                else { this.IsRepeated = Convert.ToBoolean(value); }
        //}

        /// <summary>
        /// This can be the amount the customer paid (for validation request) 
        /// or the amount that goes to Biller’s account after transaction fee is subtracted 
        /// from the amount paid by customer (for notification request).
        /// </summary>
        [XmlIgnore]
        public decimal Amount
        { get; set; }

        /// <summary>
        /// String value of Sum of the transaction fee and amount of the transaction that goes to the biller’s Bank account
        /// </summary>
        [XmlElement(ElementName = "TotalAmount")]
        public string STotalAmount
        {
            get { return this.TotalAmount.ToString(); }
            set { this.TotalAmount = Convert.ToDecimal(value); }
        }


        /// <summary>
        /// Sum of the transaction fee and amount of the transaction that goes to the biller’s Bank account
        /// </summary>
        [XmlIgnore]
        public decimal TotalAmount { get; set; }


        /// <summary>
        /// String value of fee
        /// </summary>
        [XmlElement(ElementName = "Fee")]
        public string SFee
        {
            get { return this.Fee.ToString(); }
            set { this.Fee = Convert.ToDecimal(value); }
        }


        /// <summary>
        /// Transaction fee of the transaction
        /// </summary>
        [XmlIgnore]
        public decimal Fee { get; set; }

        /// <summary>
        /// Defines who pays the transaction fee. The values are: ⦁	Biller ⦁	 Customer
        /// </summary>
        public string TransactionFeeBearer { get; set; }

        /// <summary>
        /// Defines the type of transaction fee. The allowable values are: ⦁ Fixed ⦁ Percentage
        /// </summary>
        public string SplitType { get; set; }

        /// <summary>
        /// Unique code that identifies the Bank of the Biller where the payment will go to
        /// </summary>
        public string DestinationBankCode { get; set; }

        /// <summary>
        /// A narration for the transaction
        /// </summary>
        public string Narration { get; set; }

        /// <summary>
        /// A unique reference that identifies a transaction. 
        /// Please don’t rely on this, use the SessionId element to uniquely identify a transaction.
        /// </summary>
        public string PaymentReference { get; set; }

        /// <summary>
        /// Date and time when the transaction was initiated in milliseconds.
        /// </summary>
        public string TransactionInitiatedDate { get; set; }

        /// <summary>
        /// Date and time when the transaction was approved in milliseconds.
        /// </summary>
        public string TransactionApprovalDate { get; set; }

        /// <summary>
        /// List of params
        /// </summary>
        [XmlElement(ElementName = "Param")]
        public List<Param> Param { get; set; }
    }
}
