using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Core.HelperModels
{
    public class USSDValidateDocumentVM
    {
        public string DocumentNumber { get; set; }

        public string ApplicantName { get; set; }

        public string ServiceName { get; set; }

        public string ApprovalDate { get; set; }
    }
}