using System;

namespace Parkway.CBS.Core.Models
{
    public class NotificationMessageItems : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        /// <summary>
        /// Field name
        /// </summary>
        public virtual string KeyName { get; set; }

        /// <summary>
        /// Field value
        /// </summary>
        public virtual string Value { get; set; }

        public virtual NotificationMessage NotificationMessage { get; set; }
    }
}