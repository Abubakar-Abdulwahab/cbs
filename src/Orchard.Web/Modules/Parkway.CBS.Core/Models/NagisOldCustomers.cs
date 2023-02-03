using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Models
{
    public class NagisOldCustomers : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual TaxEntity TaxEntity { get; set; }

        public virtual NagisDataBatch NagisDataBatch { get; set; }

        public virtual string CustomerName { get; set; }

        public virtual int TaxEntityCategory_Id { get; set; }

        public virtual string Address { get; set; }

        public virtual string PhoneNumber { get; set; }

        public virtual string CustomerId { get; set; }

        public virtual string TIN { get; set; }
    }
}