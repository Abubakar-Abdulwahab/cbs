using System;
using System.Collections.Generic;

namespace Parkway.CBS.Core.HelperModels
{
    public class PAYEScheduleListVM
    {
        public HeaderObj HeaderObj { get; set; }

        public string BatchRef { get; set; }

        public string DateFilter { get; set; }

        public IEnumerable<PAYEBatchRecordVM> BatchRecords { get; set; }

        public Int64 DataSize { get; set; }

        public string Token { get; set; }
    }
}