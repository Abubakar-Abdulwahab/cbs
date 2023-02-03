using System.Collections.Generic;

namespace Parkway.CBS.Core.HelperModels
{
    public class PAYEBatchItemsListVM
    {
        public HeaderObj HeaderObj { get; set; }

        public string BatchRef { get; set; }

        public IEnumerable<PAYEBatchItemsVM> BatchItems { get; set; }

        public int DataSize { get; set; }
    }
}