using Hangfire;
using Parkway.CBS.ClientRepository;
using Parkway.CBS.ClientRepository.Repositories.ReferenceDataImpl;
using Parkway.CBS.ClientRepository.Repositories.ReferenceDataImpl.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.HangFireInterface.Configuration;
using Parkway.CBS.Services.Implementations.Contracts;
using Parkway.CBS.Services.Logger;
using Parkway.CBS.Services.Logger.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Services.Implementations
{
    public class BatchInvoiceResponseEntry : IBatchInvoiceResponseEntry
    {
        private static readonly ILogger log = new ServicesLog4netLogger();

        public IUoW UoW { get; set; }

        public IGeneralBatchReferenceDAOManager GeneralBatchReferenceDAO { get; set; }

        protected void SetBatchDAO()
        { if (GeneralBatchReferenceDAO == null) { GeneralBatchReferenceDAO = new GeneralBatchReferenceDAOManager(UoW); }
        }

        protected void SetUnitofWork(string tenantName)
        {
            if (UoW == null)
            {
                UoW = new UoW(tenantName + "_SessionFactory", "ClientFileServices");
            }
        }

        public void CallImplementingClass(string tenantName, string generalBatchReferenceId, string batchFileName)
        {
            log.Info($"About to queue batch invoice response to the Hangfire. Batch Identifier {generalBatchReferenceId}");
            try
            {
                //set unit of work
                SetUnitofWork(tenantName);

                long batchId = 0;
                bool parsed = Int64.TryParse(generalBatchReferenceId, out batchId);
                if (!parsed)
                {
                    throw new Exception("Unable to convert the General Reference ID");
                }

                //instantiate the ReferenceDataBatch repository
                SetBatchDAO();
                GeneralBatchReference batchRecord = GeneralBatchReferenceDAO.GetBatchRecord(batchId);

                var nameSplit = batchRecord.AdapterClassName.Split(',');
                var implementingClass = Activator.CreateInstance(nameSplit[1].Trim(), nameSplit[0].Trim());
                var impl = ((IBatchInvoiceResponseProcessor)implementingClass.Unwrap());

                StartHangfireServer(tenantName);
                BackgroundJob.Enqueue(() => impl.GetCashFlowBatchInvoiceResponse(tenantName, batchRecord.Id, batchFileName));
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
