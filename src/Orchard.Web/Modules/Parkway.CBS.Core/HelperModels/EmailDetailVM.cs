using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class EmailDetailVM
    {
        public int ApprovalStatus { get; set; }

        public string ApprovalNumber { get; set; }

        public string TemplateName { get; set; }

        public string Subject { get; set; }

        public string Comment { get; set; }

        public string InvoiceNumber { get; set; }

        public string RequestType { get; set; }

        public string RequestDate { get; set; }
    }
}