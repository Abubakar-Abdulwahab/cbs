using Hangfire.Common;
using Hangfire.States;
using Hangfire.Storage;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Parkway.CBS.HangFireInterface.Configuration
{
    public class HangFireScheduler
    {
        public static string GetConnectionString(string tenantName)
        {
            var xmlstring = (ConfigurationManager.GetSection(typeof(HangFireSchedulerCollection).Name) as string);

            XmlSerializer serializer = new XmlSerializer(typeof(HangFireSchedulerCollection));

            HangFireSchedulerCollection schedulerItems = new HangFireSchedulerCollection();

            using (StringReader reader = new StringReader(xmlstring))
            {
                schedulerItems = (HangFireSchedulerCollection)serializer.Deserialize(reader);
            }
            if (schedulerItems == null)
            {
                return "";
            }

            return schedulerItems.HangFireSchedulerItem.Where(x => x.TenantName.Equals(tenantName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().ConnectionString;
        }
    }

    public class ProlongExpirationTimeAttribute : JobFilterAttribute, IApplyStateFilter
    {

        public void OnStateUnapplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
        {
            var value = ConfigurationManager.AppSettings["HangfireJobExpirationDay"];
            double getHangfireExpirationDay = 7d;

            if (!string.IsNullOrEmpty(value))
            {
                bool parsed = double.TryParse(value, out getHangfireExpirationDay);
                if (!parsed) { getHangfireExpirationDay = 7d; }
            }
            context.JobExpirationTimeout = TimeSpan.FromDays(getHangfireExpirationDay);
        }

        public void OnStateApplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
        {
            var value = ConfigurationManager.AppSettings["HangfireJobExpirationDay"];
            double getHangfireExpirationDay = 7d;

            if (!string.IsNullOrEmpty(value))
            {
                bool parsed = double.TryParse(value, out getHangfireExpirationDay);
                if (!parsed) { getHangfireExpirationDay = 7d; }
            }

            context.JobExpirationTimeout = TimeSpan.FromDays(getHangfireExpirationDay);
        }
    }
}
