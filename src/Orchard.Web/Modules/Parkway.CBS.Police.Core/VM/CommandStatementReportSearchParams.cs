using System;

namespace Parkway.CBS.Police.Core.VM
{
    public class CommandStatementReportSearchParams
    {
        public bool DontPageData { get; set; }

        public DateTime EndDate { get; set; }

        public int Skip { get; set; }

        public DateTime StartDate { get; set; }

        public int Take { get; set; }

        public string TransactionReference { get; set; }

        public int TransactionTypeId { get; set; }

        public DateTime? ValueDate { get; set; }

        public string CommandCode { get; set; }
    }
}