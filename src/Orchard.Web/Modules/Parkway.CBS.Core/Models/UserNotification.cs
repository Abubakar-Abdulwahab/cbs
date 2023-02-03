using System;

namespace Parkway.CBS.Core.Models
{
    public class UserNotification : CBSBaseModel
    {
        public virtual long Id { get; set; }

        public virtual bool IsDeleted { get; set; }

        public virtual int MessageCount { get; set; }

        public virtual CBSUser User { get; set; }

    }
}