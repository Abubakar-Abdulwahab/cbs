using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using Hangfire;
using Parkway.CBS.HangFireInterface.Configuration;
using Parkway.CBS.HangFireInterface.Notification.Contracts;
using Parkway.CBS.HangFireInterface.Notification.RemoteClient;
using Parkway.CBS.HangFireInterface.Notification.RemoteClient.Contracts;

namespace Parkway.CBS.HangFireInterface.Notification
{
    public class APINotification : IHangfireNotification
    {
        private IClient _remoteClient;

        [ProlongExpirationTime]
        public string ScheduleNewNotification(string tenantName, string model, string callBackURL, Dictionary<string, string> queryStringParameters)
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

                _remoteClient = new Client();
                return BackgroundJob.Enqueue(() => _remoteClient.SendRequest(model, callBackURL, HttpMethod.Post, queryStringParameters));
            }
            catch (Exception ex)
            {
                throw new Exception($"Unable to queue the job for tenant name {tenantName}. Exception=> {ex}");
            }
        }

        public bool ScheduleNewNotification(string model, string callBackURL, Dictionary<string, string> queryStringParameters)
        {
            try
            {
                _remoteClient = new Client();
                _remoteClient.SendRequest(model, callBackURL, HttpMethod.Post, queryStringParameters);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Unable to send notification. CallBack URL::: {callBackURL}. Exception=> {ex}");
            }
        }
    }
}
