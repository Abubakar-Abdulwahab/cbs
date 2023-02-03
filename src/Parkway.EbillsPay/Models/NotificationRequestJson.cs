using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.EbillsPay.Models
{
    public class NotificationRequestJson
    {
        /// <summary>
        /// Uniquely identifies a transaction
        /// </summary>
        public string SessionID { get; set; }

        /// <summary>
        /// Name of the Bank where the payment was made
        /// </summary>
        public string BankName { get; set; }

        /// <summary>
        /// Name of the Bank branch where the payment was made
        /// </summary>
        public string BranchName { get; set; }

        /// <summary>
        /// Identifies the channel where the transaction was initiated. This can be internet banking, ATM etc.
        /// </summary>
        public string ChannelCode { get; set; }

        /// <summary>
        /// Name of the biller
        /// </summary>
        public string BillerName { get; set; }

        /// <summary>
        /// Unique identifier of the product
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// Sum of the transaction fee and amount of the transaction that goes to the biller’s Bank account
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Unique code that identifies the Bank of the Biller where the payment will go to
        /// </summary>
        public string DestinationInstitutionCode { get; set; }

        public string TransactionTime { get; set; }

        /// <summary>
        /// Date and time when the transaction was approved in milliseconds.
        /// </summary>
        public string TransactionDate { get; set; }

        /// <summary>
        /// Params object
        /// </summary>
        public Params Params { get; set; }
    }
}
