using System;

namespace Parkway.CBS.Police.Core.VM
{
    public class PSSSettlementBatchItemsVM
    {
        public DateTime TransactionDate { get; set; }

        public string FileNumber { get; set; }

        public string InvoiceNumber { get; set; }

        public string CustomerName { get; set; }

        public string ServiceName { get; set; }

        public string RevenueHead { get; set; }

        public decimal SettlementAmount { get; set; }

        public decimal InvoiceAmount { get; set; }

        public string SettlementParty { get; set; }

        public string SettlementBatchId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SettlementBatchRef { get; set; }

        public string GeneratedByCommandName { get; set; }

        public string AdapterValue { get; set; }
    }
}