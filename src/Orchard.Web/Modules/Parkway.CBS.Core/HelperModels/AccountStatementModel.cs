using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class AccountStatementModel
    {
        public IEnumerable<AccountStatementLogModel> Report { get; set; }
        public decimal TotalCreditAmount { get; set; }
        public decimal TotalBillAmount { get; set; }

    }
    public class AccountStatementLogModel
    {
        public string ReceiptNumber { get; set; }
        public string InvoiceNumber { get; set; }
        public decimal Amount { get; set; }
        public decimal BillAmount { get; set; }
        public int TypeID { get; set; }
    }

    public class AccountStatementAggregate
    {
        public decimal TotalCreditAmount { get; set; }
        public decimal TotalBillAmount { get; set; }
    }
}