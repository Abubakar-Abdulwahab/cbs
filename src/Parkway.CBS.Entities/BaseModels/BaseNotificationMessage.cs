using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Entities.BaseModels
{
    public abstract class BaseNotificationMessage  : BaseCBSModel
    {
        public virtual Int64 Id { get; set; }

        public virtual Int32 MDA_Id { get; set; }

        public virtual Int32 RevenueHead_Id { get; set; }

        public virtual Int32 TaxPayer_Id { get; set; }

        public virtual Int32 NotificationTypeId { get; set; }

        public virtual string Recipient { get; set; }

        public virtual string MailSubject { get; set; }

        public virtual string Body { get; set; }

        public virtual Int32 DeliveryStatusId { get; set; }

        public virtual DateTime SentDate { get; set; }
    }
}
