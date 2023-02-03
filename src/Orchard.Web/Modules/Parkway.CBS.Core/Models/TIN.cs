using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Models
{
    public class TIN : CBSModel
    {
        public virtual string TINNumber { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string DOB { get; set; }
        public virtual string Nationality { get; set; }
        public virtual string State { get; set; }
        public virtual string Address { get; set; }
        public virtual string Occupation { get; set; }
        public virtual string CompanyName { get; set; }
        public virtual string PhoneNumber { get; set; }
        public virtual string RCNumber { get; set; }
        public virtual string Email { get; set; }
        public virtual int CustomerType { get; set; }
        public virtual TaxEntityCategory TaxEntityCategory { get; set; }
        public int RevenueHeadId { get; set; }

    }
}

