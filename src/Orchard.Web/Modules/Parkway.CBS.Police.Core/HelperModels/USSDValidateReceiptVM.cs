using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Core.HelperModels
{
    public class USSDValidateReceiptVM
    {
        public string ReceiptNumber { get; set; }

        public string ApplicantName { get; set; }

        public string ServiceName { get; set; }

        public string AmountPaid { get; set; }

        public string PaymentDate { get; set; }
    }
}