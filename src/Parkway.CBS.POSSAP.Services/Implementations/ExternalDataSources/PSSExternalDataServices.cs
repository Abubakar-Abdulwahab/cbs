using Hangfire;
using Parkway.CBS.ClientRepository;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.HangFireInterface.Configuration;
using Parkway.CBS.HangFireInterface.Logger;
using Parkway.CBS.HangFireInterface.Logger.Contracts;
using Parkway.CBS.POSSAP.Services.HelperModel;
using Parkway.CBS.POSSAP.Services.Implementations.ExternalDataSources.Contracts;
using Parkway.CBS.POSSAP.Services.PSSRepositories;
using Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace Parkway.CBS.POSSAP.Services.Implementations.ExternalDataSources
{
    public class PSSExternalDataServices
    {
        private static readonly ILogger log = new Log4netLogger();

        public IUoW UoW { get; set; }

        protected void SetUnitofWork(string tenantName)
        {
            if (UoW == null)
            {
                UoW = new UoW(tenantName.Replace(" ", "") + "_SessionFactory", "PSSExternalDataJob");
            }
        }

        public IExternalDataSourceConfigurationSettingDAOManager externalDataSourceConfigurationSettingDAOManager { get; set; }

        private void SetExternalDataSourceConfigurationSettingDAOManager()
        {
            if (externalDataSourceConfigurationSettingDAOManager == null) { externalDataSourceConfigurationSettingDAOManager = new ExternalDataSourceConfigurationSettingDAOManager(UoW); }
        }

        public string BeginExternalDataProcessing(string tenantName)
        {
            ////Do checking and queue each job
            //where we call the database to get the schedule for each task
            //depending on if task is due, then queue like this

            log.Info($"About to start external data source jobs schedule");
            try
            {
                SetUnitofWork(tenantName);
                SetExternalDataSourceConfigurationSettingDAOManager();

                DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 0, 0);
                Int64 recordCount = externalDataSourceConfigurationSettingDAOManager.Count(x => x.IsActive && x.NextScheduleDate >= today && x.NextScheduleDate <= today.AddHours(6));
                if (recordCount < 1) return "No active external data source to be processed";

                var chunkSizeStringValue = ConfigurationManager.AppSettings["ChunkSize"];
                int chunkSize = 100;

                if (!string.IsNullOrEmpty(chunkSizeStringValue))
                {
                    int.TryParse(chunkSizeStringValue, out chunkSize);
                }

                //get the pages
                int pages = Util.Pages(chunkSize, recordCount);
                int stopper = 0;
                int skip = 0;
                StartHangfireServer();
                List<PSSExternalDataSourceConfigurationSettingVM> externalDataSources = null;
                while (stopper < pages)
                {
                    externalDataSources = externalDataSourceConfigurationSettingDAOManager.GetBatchActiveExternalDataSourceConfigurations(chunkSize, skip, today);
                    foreach (PSSExternalDataSourceConfigurationSettingVM externalDataSource in externalDataSources)
                    {
                        log.Info($"About to check external data source {externalDataSource.ActionName} configured next schedule date. The next schedule date is {externalDataSource.NextScheduleDate}. External data source config Id: {externalDataSource.ExternalDataSourceConfigId}");
                      
                        var nameSplit = externalDataSource.ImplementingClass.Split(',');
                        var implementingClass = Activator.CreateInstance(nameSplit[1].Trim(), nameSplit[0].Trim());
                        var impl = ((IPSSExternalDataSourceProcessor)implementingClass.Unwrap());

                        log.Info($"About to queue external data source {externalDataSource.ActionName} for processing. External data source config Id: {externalDataSource.ExternalDataSourceConfigId}");
                        BackgroundJob.Schedule(() => impl.ProcessExternalDataSource(tenantName, externalDataSource), externalDataSource.NextScheduleDate);
                        log.Info($"External data source {externalDataSource.ActionName} queued successfully. External data source config Id: {externalDataSource.ExternalDataSourceConfigId}");
                    }
                    externalDataSources.Clear();
                    skip += chunkSize;
                    stopper++;
                }

                log.Info($"External data source job queued!!!");
                return "External data source job queued!!!";
            }
            catch (Exception exception)
            {
                log.Error($"Error processing external data source configurations");
                log.Error(exception.Message, exception);
                throw;
            }
        }

        private void StartHangfireServer()
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
        }
    }
}
