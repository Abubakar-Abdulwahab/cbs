using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CBSPay.Core.APIModels
{
    public class APIResponse
    {
        public bool Success { get; set; } 
        public string ErrorMessage { get; set; }
        public dynamic Result { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }

    public class EIRSAPIResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public dynamic Result { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }

    public class PaymentStatusModel
    {
        public bool Success { get; set; }
        public string TaxPayerName { get; set; }
        public decimal TotalAmountPaid {get;set;}
        public string ReferenceNumber { get; set; } 
        public string Message { get; set; }
        public string RIN { get; set; }

    }
}



