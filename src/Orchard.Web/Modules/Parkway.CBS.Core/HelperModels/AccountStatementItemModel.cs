using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class AccountStatementItemModel
    {
        /// <summary>
        /// Gets or sets the description or narration of the transaction.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the transaction Reference No.
        /// </summary>
        /// <value>The ref no.</value>
        public string ReferenceNo { get; set; }

        /// <summary>
        /// Gets or sets the Transaction Date.
        /// </summary>
        /// <value>The Transaction Date.</value>
        public DateTime TransactionDate { get; set; }

        /// <summary>
        /// Gets or sets the value date.
        /// </summary>
        /// <value>The value date.</value>
        public DateTime ValueDate { get; set; }

        /// <summary>
        /// Gets or sets the transaction amount.
        /// </summary>
        /// <value>The TransAmount.</value>
        public decimal TransAmount { get; set; }

        public TransactionType TransactionType { get; set; }
    }
}