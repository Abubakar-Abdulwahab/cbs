using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBSPay.Core.APIModels
{
    /// <summary>
    /// contains details of the taxpayer and payment to be sent to the bank(in branch) system
    /// </summary>
    public class TaxPayerPaymentInfo
    {
        public string ReferenceNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string TaxPayerName { get; set; }
        public decimal TotalAmountPaid { get; set; }
        /// <summary>
        /// /synonymous to total, price
        /// </summary>
        public decimal ReferenceAmount { get; set; }
        public string TaxPayerTypeName { get; set; }
        public string RIN { get; set; }
        public DateTime ReferenceDate { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public int Quantity { get; set; }
        public decimal Tax { get; set; }
    }
}
