using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.ReferenceData.Admin.ViewModels
{
    public class NAGISDataBatchVM
    {

        public string BatchRef { get; set; }

        public IEnumerable<NAGISDataBatchCollectionDetails> ReportRecords { get; set; }
    }
}