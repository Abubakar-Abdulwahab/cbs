using CBSPay.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBSPay.Core.APIModels
{
    public class POATaxPayerResponse 
    {
        public long TaxPayerTypeID { get; set; }
        public long TaxPayerID { get; set; }
        public string TaxPayerRIN { get; set; }
        public string TaxPayerName { get; set; }
        public string TaxPayerTypeName { get; set; }
        public string TaxPayerMobileNumber { get; set; }
        public string TaxPayerAddress { get; set; }
        public string Status { get; set; }
    }
}
