using System;

namespace Parkway.CBS.Core.Models
{
    public class NotificationMessage : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual TaxEntity TaxPayer { get; set; }

        public virtual Int32 NotificationTypeId { get; set; }

        public virtual string Recipient { get; set; }

        public virtual Int32 DeliveryStatusId { get; set; }

        public virtual DateTime SentDate { get; set; }
    }
}