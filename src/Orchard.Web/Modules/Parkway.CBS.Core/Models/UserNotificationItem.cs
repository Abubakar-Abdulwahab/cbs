using System;

namespace Parkway.CBS.Core.Models
{
    public class UserNotificationItem : CBSBaseModel
    {
        public virtual long Id { get; set; }

        public virtual bool IsRead { get; set; }

        public virtual bool IsDeleted { get; set; }

        public virtual string Message { get; set; }

        public virtual DateTime ReadAt { get; set; }

        public virtual UserNotification UserNotification { get; set; }

    }
}