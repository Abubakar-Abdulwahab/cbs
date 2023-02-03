using System;
using Orchard.Users.Models;

namespace Parkway.CBS.Core.Models
{
    public class PaymentProviderValidationConstraint : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual MDA MDA { get; set; }

        public virtual RevenueHead RevenueHead { get; set; }

        public virtual ExternalPaymentProvider PaymentProvider { get; set; }

        public virtual bool IsDeleted { get; set; }

        public virtual UserPartRecord LastUpdatedBy { get; set; }
    }
}