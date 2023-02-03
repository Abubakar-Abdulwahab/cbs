using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Module.Web.ViewModels
{
    public class PaymentsVM
    {
        public long TaxEntityId { get; set; }

        public HeaderObj HeaderObj { get; set; }

        public ReceiptStatus ReceiptStatus { get; set; }

        public string ReceiptNumber { get; set; }

        public string DateFilter { get; set; }

        public Int64 DataSize { get; set; }

        public string Token { get; set; }

        public int TotalNumberOfPayment { get; set; }

        public decimal TotalAmountPaid { get; set; }

        public string SelectedBank { get; set; }

        public List<BankVM> Banks { get; set; }

        public IEnumerable<CollectionDetailReport> ReportRecords { get; set; }

    }
}