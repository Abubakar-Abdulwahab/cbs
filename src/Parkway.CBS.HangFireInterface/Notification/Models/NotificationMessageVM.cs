using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.HangFireInterface.Notification.Models
{
    public class NotificationMessageVM
    {
        public virtual string Recipient { get; set; }

        public virtual string Subject { get; set; }

        public virtual string Body { get; set; }
    }
}
