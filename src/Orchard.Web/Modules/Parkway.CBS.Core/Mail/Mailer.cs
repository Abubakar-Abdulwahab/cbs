using Orchard;
using Orchard.ContentManagement;
using Orchard.Email.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace Parkway.CBS.Core.Mail
{
    public abstract class Mailer : IDisposable
    {
        protected readonly SmtpSettingsPart _smtpSettings;
        private readonly Lazy<SmtpClient> _smtpClientField;

        public Mailer(IOrchardServices orchardServices)
        {
            _smtpSettings = orchardServices.WorkContext.CurrentSite.As<SmtpSettingsPart>();
            _smtpClientField = new Lazy<SmtpClient>(CreateSmtpClient);
        }

        public void Dispose()
        {
            if (!_smtpClientField.IsValueCreated)
            {
                return;
            }
            _smtpClientField.Value.Dispose();
        }

        public bool SendEmail(MailMessage mail)
        {
            //if (!_smtpSettings.IsValid())
            //{
            //    return false;
            //}
            //_smtpClientField.Value.Send(mail);
            return true;
        }

        private SmtpClient CreateSmtpClient()
        {
            // If no properties are set in the dashboard, use the web.config value.
            if (String.IsNullOrWhiteSpace(_smtpSettings.Host))
            {
                return new SmtpClient();
            }

            var smtpClient = new SmtpClient
            {
                UseDefaultCredentials = _smtpSettings.RequireCredentials && _smtpSettings.UseDefaultCredentials
            };

            if (!smtpClient.UseDefaultCredentials && !String.IsNullOrWhiteSpace(_smtpSettings.UserName))
            {
                smtpClient.Credentials = new NetworkCredential(_smtpSettings.UserName, _smtpSettings.Password);
            }

            if (_smtpSettings.Host != null)
            {
                smtpClient.Host = _smtpSettings.Host;
            }

            smtpClient.Port = _smtpSettings.Port;
            smtpClient.EnableSsl = _smtpSettings.EnableSsl;
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            return smtpClient;
        }        
    }
}