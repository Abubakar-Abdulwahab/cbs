using System.Collections.Generic;

namespace Parkway.CBS.Core.HelperModels
{
    public class ReceiptVM
    {
        public string ReceiptNumber { get; set; }

        public long Id { get; set; }

        /// <summary>
        /// hold the list of transaclogs
        /// </summary>
        internal List<TransactionLogVM> TransactionLogs { get; set; }
    }
}