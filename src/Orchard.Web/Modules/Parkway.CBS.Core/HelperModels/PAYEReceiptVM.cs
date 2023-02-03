using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class PAYEReceiptVM
    {
        public Int64 Id { get; set; }

        public IEnumerable<TransactionLogVM> TransactionLogs { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal AvailableAmount { get { return TotalAmount - UtilizedAmount; } }

        public string InvoiceNumber { get; set; }

        public decimal UtilizedAmount { get; set; }

        public decimal UtilizedAmountForSchedule { get; set; }

        public long ReceiptId { get; set; }

        public string ReceiptNumber { get; set; }

        public int Status { get; set; }

        public DateTime PaymentDate { get; set; }

        public string PaymentDateStringVal { get { return PaymentDate.ToString("dd MMM yyyy"); } }

        public string UtilzationStatus { get { return ((PAYEReceiptUtilizationStatus)Status).ToDescription(); } }

        public string PayerName { get; set; }

        public string PayerId { get; set; }

        public string PhoneNo { get; set; }
    }
}