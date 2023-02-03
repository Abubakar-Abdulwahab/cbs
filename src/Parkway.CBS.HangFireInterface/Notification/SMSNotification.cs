using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Hangfire;
using InfoGRID.Pulse.SDK;
using InfoGRID.Pulse.SDK.DTO;
using InfoGRID.Pulse.SDK.Utils;
using Parkway.CBS.HangFireInterface.Configuration;
using Parkway.CBS.HangFireInterface.Notification.Contracts;
using Parkway.CBS.HangFireInterface.Utilities;
using Parkway.CBS.HangFireInterface.Utilities.Enums;

namespace Parkway.CBS.HangFireInterface.Notification
{
    public class SMSNotification : ISMSNotification
    {
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
