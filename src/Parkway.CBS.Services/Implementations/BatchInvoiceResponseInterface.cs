using Hangfire;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.HangFireInterface.Configuration;
using Parkway.CBS.Services.Implementations.Contracts;
using Parkway.CBS.Services.Logger;
using Parkway.CBS.Services.Logger.Contracts;
using System;

namespace Parkway.CBS.Services.Implementations
{
    public class BatchInvoiceResponseInterface : IBatchInvoiceResponseInterface
    {
        private static readonly ILogger log = new ServicesLog4netLogger();

        public void ProcessInvoices(string tenantName, string batchIdentifier, string batchFileName)
        {
            log.Info($"About to queue batch invoice response to the Hangfire");
            try
            {
                IBatchInvoiceResponseEntry batchInvoiceEntry = new BatchInvoiceResponseEntry();

                StartHangfireServer(tenantName);
                BackgroundJob.Enqueue(() => batchInvoiceEntry.CallImplementingClass(tenantName, batchIdentifier, batchFileName));                
            }
            catch (Exception exception)
            {
                log.Error(string.Format("Error while queuing batch invoice response to the Hangfire for stage {0}", ReferenceDataProcessingStages.UpdateInvoiceStagingWithCashFlowResponse));
                log.Error(exception.Message, exception);
                throw;
            }
        }

        private void StartHangfireServer(string tenantName)
        {
            //Get the connection string
            string dbConnectionString = HangFireScheduler.GetConnectionString(tenantName);
            //Call the Hangfire storage
            GlobalConfiguration.Configuration.UseSqlServerStorage(dbConnectionString);
        }

    }
}
