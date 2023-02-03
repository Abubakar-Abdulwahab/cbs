using CBSPay.Core.APIModels;
using CBSPay.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBSPay.Core.ViewModels
{
    public class POASearchResponse
    {
        public IEnumerable<POATaxPayerResponse> TaxPayerResponses { get; set; }
        public string Status { get; set; }
        public IEnumerable<EconomicActivities> EconomicActivities { get; set; }
    }
}
