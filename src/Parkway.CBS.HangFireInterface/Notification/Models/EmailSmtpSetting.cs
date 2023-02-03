using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.HangFireInterface.Notification.Models
{
    public class EmailSmtpSetting
    {
        public string SmtpHost { get; set; }

        public string MailUsername { get; set; }

        public string MailPassword { get; set; }

        public int SmtpPort { get; set; }

        public bool UseSSL { get; set; }

        public string BaseURL { get; set; }
    }
}
