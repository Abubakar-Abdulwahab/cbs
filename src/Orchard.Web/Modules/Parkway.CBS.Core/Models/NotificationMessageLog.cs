using System;

namespace Parkway.CBS.Core.Models
{
    public class NotificationMessageLog : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        /// <summary>
        /// <see cref="Enums.NotificationMessageType"/>
        /// </summary>
        public virtual int NotificationType { get; set; }

        public virtual string Reference { get; set; }
    }
}