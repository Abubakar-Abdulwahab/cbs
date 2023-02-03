using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class TransactionLogInvoiceStatusVM
    {
        public decimal AmountPaid { get; set; }

        public DateTime PaymentDate { get; set; }

        public string Bank { get; set; }

        public string BankCode { get; set; }

        public string BankBranch { get; set; }

        public string Channel { get; set; }

        /// <summary>
        /// <see cref="Enums.PaymentProvider"/>
        /// </summary>
        public string PaymentProvider { get; set; }

        /// <summary>
        /// Payment ref here is the ref sent to the payment processor
        /// </summary>
        public string PaymentReference { get; set; }

        public DateTime? TransactionDate { get; set; }

    }
}