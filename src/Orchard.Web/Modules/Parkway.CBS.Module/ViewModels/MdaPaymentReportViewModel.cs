using Parkway.CBS.Core.HelperModels;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Parkway.CBS.Module.ViewModels
{
    public class MdaPaymentReportViewModel //dd
    {
        public IEnumerable<SelectListItem> MdaSelected { get; set; }
        public IEnumerable<SelectListItem> RevenueHeadSelected { get; set; }

        public string FromRange { get; set; }
        public string EndRange { get; set; }

        public PaymentOptions Options { get; set; }

    }

    //public class PaymentOptions
    //{
    //    public InvoiceStatusList PaymentStatus { get; set; }
    //}
}