using Orchard.Email.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.UI.WebControls;

namespace Parkway.CBS.Core.Mail
{
    public class MailerBuilder : IMailerBuilder
    {
        private MailMessage mail;
        public MailerBuilder()
        {
            mail = new MailMessage();
            mail.IsBodyHtml = true;
        }

        public void To(Dictionary<string, string> emails)
        {
            EmailMessage g = new EmailMessage();
            foreach (var email in emails)
            {
                mail.To.Add(new MailAddress(email.Key, email.Value));
            }
        }

        public void From(string address="", string sender="")
        {
            if (string.IsNullOrEmpty(address))
            {
                mail.From = new MailAddress("cbs@parkwayprojects.com", "Parkway Admin");
            }else
            {
                mail.From = new MailAddress(address, sender);
            }
        }

        public void Subject(string subject)
        {
            mail.Subject = subject;
        }

        public void Body(string body, ListDictionary data)
        {
            MailDefinition definition = new MailDefinition();
            definition.From = mail.From.Address;
            mail = definition.CreateMailMessage(string.Join(",", mail.To), data, body, new System.Web.UI.Control());
        }

        public MailMessage GetMessage()
        {
            return mail;
        }
    }
}