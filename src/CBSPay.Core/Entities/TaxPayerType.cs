using CBSPay.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBSPay.Core.Entities
{
    public class TaxPayerType : BaseEntity<int>
    {
        public virtual int TaxPayerTypeID { get; set; }
        public virtual string TaxPayerTypeName { get; set; }
        public virtual bool Active { get; set; }
        public virtual string ActiveText { get; set; }
    }
}
