using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.ReferenceData.Admin.ViewModels
{
    public class ReferenceDataBatchVM
    {
        public dynamic Pager { get; set; }

        public string BatchRef { get; set; }

        public IEnumerable<ReferenceDataBatchCollectionDetails> ReportRecords { get; set; }

        public int TotalNumberOfRecords { get; set; }

        public string FromRange { get; set; }

        public string EndRange { get; set; }
    }
}