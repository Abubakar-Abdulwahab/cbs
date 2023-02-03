using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class PAYEReceiptUtilizationReportVM
    {
        public string BatchRef { get; set; }

        public decimal UtilizedAmount { get; set; }

        public DateTime UtilizedDate { get; set; }
    }
}