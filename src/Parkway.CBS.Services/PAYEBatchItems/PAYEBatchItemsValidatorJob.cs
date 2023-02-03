using Hangfire;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.HangFireInterface.Configuration;
using Parkway.CBS.Services.PAYEBatchItems.Contracts;
using System;

namespace Parkway.CBS.Services.PAYEBatchItems
{
    public class PAYEBatchItemsValidatorJob : IPAYEBatchItemsValidatorJob
    {

        /// <summary>
        /// Queues items for validation by the batch record Id using hangfire
        /// </summary>
        /// <param name="batchStagingRecordId">Batch record Id for the collection of items to be validated</param>
        /// <param name="tenantName">Tenant name</param>
        public void ValidateItemsByBatchRecordId(string tenantName, long batchStagingRecordId)
        {
            StartHangfireServer();

            BackgroundJob.Enqueue(() => BeginValidationProcess());
        }

        private void StartHangfireServer()
        {
            var conStringNameKey = AppSettingsConfigurations.GetSettingsValue(AppSettingEnum.HangfireConnectionStringName);
            if (string.IsNullOrEmpty(conStringNameKey))
            {
                throw new Exception("Unable to get the hangfire connection string name");
            }

            //Get the connection string
            string dbConnectionString = HangFireScheduler.GetConnectionString(conStringNameKey);
            //Call the Hangfire storage
            GlobalConfiguration.Configuration.UseSqlServerStorage(dbConnectionString);
        }
        
        /// <summary>
        /// Begin the validation process
        /// </summary>
        public void BeginValidationProcess()
        {

        }
    }
}
