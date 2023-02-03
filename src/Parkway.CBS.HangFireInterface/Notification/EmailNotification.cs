using System;
using System.Collections.Generic;
using Hangfire;
using System.Net.Mail;
using System.Net;
using System.Configuration;
using Parkway.CBS.HangFireInterface.Notification.Contracts;
using Parkway.CBS.HangFireInterface.Configuration;
using Parkway.CBS.HangFireInterface.Notification.Models;
using Newtonsoft.Json;
using System.Web.UI.WebControls;
using System.Collections.Specialized;
using InfoGRID.Pulse.SDK.Utils;
using InfoGRID.Pulse.SDK;
using InfoGRID.Pulse.SDK.Models;
using InfoGRID.Pulse.SDK.DTO;
using Parkway.CBS.HangFireInterface.Utilities;
using System.Linq;
using Parkway.CBS.HangFireInterface.Utilities.Enums;
using System.IO;
using System.Web.Hosting;
using System.Text;
using System.Reflection;

namespace Parkway.CBS.HangFireInterface.Notification
{
    public class EmailNotification : IEmailNotification
    {
        [ProlongExpirationTime]
        public bool SendNotificationGmail(string messageVM, Dictionary<string, string> data, string templateName)
        {
            try
            {
                NotificationMessageVM notificationMessage = JsonConvert.DeserializeObject<NotificationMessageVM>(messageVM);

                var conStringName = ConfigurationManager.AppSettings["HangfireConnectionStringName"];

                if (string.IsNullOrEmpty(conStringName))
                {
                    throw new Exception("Unable to get the hangfire connection string name");
                }

                //Get the connection string
                string dbConnectionString = HangFireScheduler.GetConnectionString(conStringName);

                //Call the Hangfire storage
                GlobalConfiguration.Configuration.UseSqlServerStorage(dbConnectionString);

                BackgroundJob.Enqueue(() => SendRequest(notificationMessage, data, templateName));

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
        }

        [ProlongExpirationTime]
        public bool SendNotificationPulse(Pulse pulse, PulseHeader pulseHeader)
        {
            try
            {
                var conStringName = ConfigurationManager.AppSettings["HangfireConnectionStringName"];

                if (string.IsNullOrEmpty(conStringName))
                {
                    throw new Exception("Unable to get the hangfire connection string name");
                }

                //Get the connection string
                string dbConnectionString = HangFireScheduler.GetConnectionString(conStringName);

                //Call the Hangfire storage
                GlobalConfiguration.Configuration.UseSqlServerStorage(dbConnectionString);

                BackgroundJob.Enqueue(() => SendRequest(pulse, pulseHeader));

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [ProlongExpirationTime]
        public bool SendRequest(NotificationMessageVM messageVM, Dictionary<string, string> data, string template)
        {
            try
            {
                Config mailConfig = ConfigUtil.GetMailConfigByProvider("gmail");
                var emailSmtp = new EmailSmtpSetting
                {
                    SmtpHost = mailConfig.Node.Where(x => x.Key == MailConfigKey.SmtpHost.ToString()).FirstOrDefault().Value,
                    SmtpPort = Convert.ToInt32(mailConfig.Node.Where(x => x.Key == MailConfigKey.SmtpPort.ToString()).FirstOrDefault().Value),
                    MailUsername = mailConfig.Node.Where(x => x.Key == MailConfigKey.Username.ToString()).FirstOrDefault().Value,
                    MailPassword = mailConfig.Node.Where(x => x.Key == MailConfigKey.Password.ToString()).FirstOrDefault().Value,
                    UseSSL = true
                };

                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EmailTemplates/", template);
                StringBuilder body = new StringBuilder();
                using (StreamReader r = new StreamReader(path))
                {
                    body.Append(r.ReadToEnd());
                }

                foreach (var item in data)
                {
                    body = body.Replace("{"+item.Key+"}", item.Value);
                }

                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(emailSmtp.MailUsername);

                mailMessage.Subject = messageVM.Subject;

                mailMessage.Body = body.ToString();

                mailMessage.IsBodyHtml = true;

                mailMessage.To.Add(new MailAddress(messageVM.Recipient));

                using (var smtp = new SmtpClient())
                {
                    smtp.Host = emailSmtp.SmtpHost;

                    smtp.UseDefaultCredentials = false;

                    smtp.Credentials = new NetworkCredential { UserName = emailSmtp.MailUsername, Password = emailSmtp.MailPassword };

                    smtp.Port = Convert.ToInt32(emailSmtp.SmtpPort);

                    smtp.EnableSsl = emailSmtp.UseSSL;

                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                    smtp.Send(mailMessage);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [ProlongExpirationTime]
        public bool SendRequest(Pulse pulse, PulseHeader pulseHeader)
        {
            try
            {
                Config mailConfig = ConfigUtil.GetMailConfigByProvider("pulse");
                PulseCredentials objCred = new PulseCredentials();
                objCred.UserName = mailConfig.Node.Where(x => x.Key == MailConfigKey.Username.ToString()).FirstOrDefault().Value;
                objCred.Password = mailConfig.Node.Where(x => x.Key == MailConfigKey.Password.ToString()).FirstOrDefault().Value;

                PulseConnection con = new PulseConnection(mailConfig.Node.Where(x => x.Key == MailConfigKey.PulseURL.ToString()).FirstOrDefault().Value, objCred);
                pulseHeader.Origin = mailConfig.Node.Where(x => x.Key == MailConfigKey.PulseOrigin.ToString()).FirstOrDefault().Value;

                con.SendPulse(pulse, pulseHeader);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
