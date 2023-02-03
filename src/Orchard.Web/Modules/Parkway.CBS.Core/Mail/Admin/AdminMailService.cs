using System;
using System.Collections.Generic;
using Orchard.Security;
using System.IO;
using System.Text;
using System.Web.Hosting;
using System.Collections.Specialized;
using Orchard;
using Orchard.Logging;
using Orchard.UI.Notify;
using System.Net.Mail;
using System.Net.Configuration;
using System.Configuration;
using Orchard.Users.Events;

namespace Parkway.CBS.Core.Mail.Admin
{
    public class AdminMailService : Mailer, IAdminMailService, IDisposable
    {
        private readonly IMailerBuilder _mailer;
        private readonly IMembershipService _membershipService;
        public ILogger Logger { get; set; }
        private readonly INotifier _notifier;

        public AdminMailService(IOrchardServices orchardServices, IMailerBuilder mailer, IMembershipService membershipService) : base(orchardServices)
        {
            _mailer = mailer;
            _membershipService = membershipService;
            Logger = NullLogger.Instance;
            _notifier = orchardServices.Notifier;
        }

        public void SendActivationEmail(UserContext context)
        {
            try
            {
                StringBuilder password = new StringBuilder();
                password.Append(Path.GetRandomFileName().Replace("f", "?").Substring(0, 8));//12 characters with . in the position 9, index 8
                
                var user = context.User;
                if (!ResetPassword(user, password.ToString())) { return; }

                var emails = new Dictionary<string, string>();
                emails.Add(user.Email, user.UserName);
                _mailer.To(emails);

                var senderAddress = !String.IsNullOrWhiteSpace(_smtpSettings.Address)
                       ? new MailAddress(_smtpSettings.Address)
                       : new MailAddress(((SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp")).From);

                _mailer.From(senderAddress.Address, "Parkway Admin");
                _mailer.Subject("Activation Email");

                string path = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "Modules", "ParkWay.CBS.Users.Management/Views/Emails/Admin", "ActivationEmail.html");
                string body = "";
                using (StreamReader r = new StreamReader(path)) { body = r.ReadToEnd(); }
                ListDictionary data = new ListDictionary();
                data.Add("{name}", user.UserName);
                data.Add("{password}", password.ToString());
                _mailer.Body(body, data);

                var mail = _mailer.GetMessage();
                mail.IsBodyHtml = true;
                if (SendEmail(mail)) { _notifier.Information(Lang.Lang.adminactivationemailnotificationsent); return; }
            }
            catch (Exception exception)
            {
                Logger.Error("EMAIL ERROR \n" + exception, exception.Message);
            }
            _notifier.Information(Lang.Lang.couldnotsendadminactivationemail);
            return;
        }

        private bool ResetPassword(IUser user, string password)
        {
            if (string.IsNullOrEmpty(user.Email)) { return false; }
            _membershipService.SetPassword(user, password);
            return true;
        }
    }
}