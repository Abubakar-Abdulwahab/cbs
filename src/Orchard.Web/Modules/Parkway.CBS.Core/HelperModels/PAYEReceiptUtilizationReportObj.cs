using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class PAYEReceiptUtilizationReportObj
    {
        public HeaderObj HeaderObj { get; set; }

        public string ReceiptNumber { get; set; }

        public IEnumerable<PAYEReceiptUtilizationReportVM> ReceiptUtilizationItems { get; set; }
    }
}