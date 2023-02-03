using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class InvoiceStatusDetailsVM
    {
        public string Recipient { get; set; }

        public string InvoiceNumber { get; set; }

        public string PhoneNumber { get; set; }

        public string TIN { get; set; }

        public string Email { get; set; }

        public decimal AmountDue { get; set; }

        public DateTime DueDate { get; set; }

        /// <summary>
        ///invoice status
        /// </summary>
        public string Status { get; set; }

        public string PayerId { get; set; }

        public string RevenueHeadCallBackURL { get; set; }

        public string CallBackURL { get; set; }

        public string RequestRef { get; set; }

        public List<TransactionLogInvoiceStatusVM> Payments { get; set; }

    }
}