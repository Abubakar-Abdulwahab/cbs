using Hangfire;
using Hangfire.Common;
using log4net;
using Microsoft.Owin.Hosting;
using Parkway.CBS.ClientServices.Settlement;
using Parkway.CBS.HangfireScheduler.Configuration.Contracts;
using Parkway.CBS.POSSAP.EGSRegularization.Implementations.RecurringInvoice;
using Parkway.CBS.POSSAP.Services.Implementations.ExternalDataSources;
using Parkway.CBS.POSSAP.Services.Implementations.PSSSettlement;
using Parkway.CBS.POSSAP.Services.Implementations.PSSSettlement.Processes;
using Parkway.CBS.Services.Implementations;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Parkway.CBS.HangfireScheduler.Configuration
{
    public class HangFireScheduler : IHangFireScheduler
    {
        private static readonly ILog log = log4net.LogManager.GetLogger("Parkway.CBS.HangfireScheduler.Configuration");

        public List<HangFireSchedulerItem> GetHangFireSchedulerItems()
        {
            try
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
                    return new List<HangFireSchedulerItem>();
                }

                return schedulerItems.HangFireSchedulerItem.Where(x => x.IsActive == true).ToList();
            }
            catch (Exception ex)
            {
                log.Error(ex.Source + ex.StackTrace + ex.Message);
                return new List<HangFireSchedulerItem>();
            }

        }

        public bool StartHangFireDashboard(string tenantName, string connectionString, string DashboardUrl)
        {
            try
            {
                log.Info($"Dashboard for url { DashboardUrl } started");

                GlobalConfiguration.Configuration.UseSqlServerStorage(connectionString);
                BackgroundJobServer _server = new BackgroundJobServer();

                WebApp.Start<Startup>(DashboardUrl);

                //This job runs at an hour interval
                var manager = new RecurringJobManager();
                manager.AddOrUpdate("POSSAP Invoice Cancellation", Job.FromExpression(() => new InvoiceCancellation().ProcessInvoiceCancellation("POSSAP")), cronExpression: Cron.HourInterval(1));
                //This job runs at 12:15am everyday
                manager.AddOrUpdate("POSSAP Activate Deployment", Job.FromExpression(() => new DeploymentActivation().ProcessDeploymentActivation("POSSAP")), cronExpression: "15 0 * * *");
                //This job runs at 12:10am everyday
                manager.AddOrUpdate("POSSAP Deactivate Deployment", Job.FromExpression(() => new DeploymentDeactivation().ProcessDeploymentDeactivation("POSSAP")), cronExpression: "10 0 * * *");

                //This job runs every 6 hours
                manager.AddOrUpdate("POSSAP External Data Processing", Job.FromExpression(() => new PSSExternalDataServices().BeginExternalDataProcessing("POSSAP")), cronExpression: Cron.HourInterval(6));

                //This job runs once in a day
                //manager.AddOrUpdate("POSSAP Settlement", Job.FromExpression(() => new PSSSettlementService().BeginSettlementProcessing("POSSAP")), cronExpression: Cron.Hourly());

                //This job runs every hour
                manager.AddOrUpdate("POSSAP Settlement", Job.FromExpression(() => new CreateBatch().BeginSettlementProcessing("POSSAP")), cronExpression: Cron.Hourly());

                //This job runs at 12:10am everyday
                manager.AddOrUpdate("POSSAP Deployment Allowance", Job.FromExpression(() => new PSSDeploymentAllowanceService().BeginDeploymentAllowanceProcessing("POSSAP")), cronExpression: "10 0 * * *");

                //This job runs at 3:00am everyday
                manager.AddOrUpdate("POSSAP Regularization Recurring Invoice", Job.FromExpression(() => new PSSRegularizationRecurringInvoiceService().BeginInvoiceProcessing("POSSAP")), cronExpression: "0 3 * * *");

                ////
                //if (tenantName == "Nasarawa")
                //{
                //    //manager.AddOrUpdate(tenantName + " Settlement Job", Job.FromExpression(() => new Settlement().DoSettlement(tenantName)), Cron.Daily());
                //    manager.AddOrUpdate(tenantName + " Settlement Job", Job.FromExpression(() => new Settlement().DoSettlement(tenantName)), Cron.Daily());
                //}               
                ////
                return true;
            }
            catch (Exception ex)
            {
                log.Info($"Issue while starting the dashboard for url: { DashboardUrl }");
                log.Error(ex.Source + ex.StackTrace + ex.Message);
                return false;
            }
        }
    }
}
