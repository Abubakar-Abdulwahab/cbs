using System;
using System.Collections.Generic;

namespace Parkway.CBS.Core.HelperModels
{
    public class ReceiptUtilizationVM
    {
        public Int64 BatchRecordId { get; set; }

        public string BatchRef { get; set; }

        public decimal ScheduleAmount { get; set; }

        public decimal AmountPaid { get; set; }

        public decimal OutstandingAmount => ScheduleAmount - AmountPaid;

        public decimal Surcharge { get; set; }

        public bool PaymentCompleted { get; set; }

        public string ScheduleType { get; set; }

        public DateTime CreatedAt { get; set; }

        public HeaderObj HeaderObj { get; set; }

        public IEnumerable<PAYEReceiptVM> UtilizedReceipts { get; set; }

        public string UnpaidInvoiceNumber { get; set; }
    }
}