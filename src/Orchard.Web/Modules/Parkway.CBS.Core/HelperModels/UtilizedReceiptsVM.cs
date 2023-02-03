using System.Collections.Generic;

namespace Parkway.CBS.Core.HelperModels
{
    public class UtilizedReceiptsVM
    {
        public HeaderObj HeaderObj { get; set; }

        public string BatchRef { get; set; }

        public IEnumerable<PAYEReceiptVM> UtilizedReceipts { get; set; }
    }
}