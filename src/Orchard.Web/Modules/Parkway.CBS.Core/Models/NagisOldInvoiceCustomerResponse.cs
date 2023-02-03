using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Models
{
    public class NagisOldInvoiceCustomerResponse : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual string BatchIdentifier { get; set; }

        public virtual Int64 TaxEntity_Id { get; set; }

        public virtual Int64 PrimaryContactId { get; set; }

        public virtual Int64 CashflowCustomerId { get; set; }
    }
}