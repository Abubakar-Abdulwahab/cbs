using CBSPay.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBSPay.Core.Entities
{
    public class TaxPayerDetails : BaseEntity<long>
    {
        public virtual  long TaxPayerID { get; set; }
        public virtual  long TaxPayerTypeID { get; set; }
        public virtual  string TaxPayerRIN { get; set; }
        public virtual  string TaxPayerTIN { get; set; }
        public virtual  string TaxPayerName { get; set; }
        public virtual  string TaxPayerTypeName { get; set; }
        public virtual  string TaxPayerMobileNumber { get; set; }
        public virtual  string TaxPayerAddress { get; set; }
    }
}
