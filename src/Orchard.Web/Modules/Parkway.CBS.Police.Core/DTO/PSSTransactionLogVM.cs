using Orchard.Users.Models;
using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Core.DTO
{
    public class PSSTransactionLogVM
    {
        public decimal AmountPaid { get; set; }

        public DateTime PaymentDate { get; set; }

        /// <summary>
        /// Payment ref here is the ref sent to the payment processor
        /// </summary>
        public string PaymentReference { get; set; }

        public string PaymentProviderName { get; set; }

        public string Channel { get; set; }

        public string Bank { get; set; }

        public string InvoiceNumber { get; set; }

        public string ReceiptNumber { get; set; }

        public string PayerName { get; set; }

        public string FileRefNumber { get; set; }

        public string RevenueHeadName { get; set; }

        public string CommandName { get; set; }
    }
}