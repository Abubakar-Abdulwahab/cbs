using System;

namespace Parkway.CBS.Core.Models
{
    /// <summary>
    /// this is a view on the database
    /// </summary>
    public class InvoiceAmountDueSummary
    {
        /// <summary>
        /// Maps to the invoice Id
        /// </summary>
        public virtual Int64 Id { get; set; }

        private decimal amountDue = 00.00m;

        /// <summary>
        /// gives the total amount that has been paid so far for this invoice
        /// </summary>
        public virtual decimal AmountDue
        {
            get
            {
                if (amountDue < 0) { return 00.00m; }
                else { return amountDue; }
            }
            set { amountDue = value; }
        }
    }
}