using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBSPay.Core.ViewModels
{
    public class GetReferenceModel
    {
        public string TaxPayerName { get; set; }
        public string ReferenceNumber { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public int PaymentChannel { get; set; }
    }
}
