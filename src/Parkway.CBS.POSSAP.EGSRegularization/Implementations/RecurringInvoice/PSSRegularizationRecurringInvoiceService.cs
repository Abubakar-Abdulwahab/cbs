using Hangfire;
using Parkway.CBS.ClientRepository;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.HangFireInterface.Logger;
using Parkway.CBS.HangFireInterface.Logger.Contracts;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.POSSAP.EGSRegularization.Implementations.RecurringInvoice.Contracts;
using Parkway.CBS.POSSAP.EGSRegularization.PSSRepositories;
using Parkway.CBS.POSSAP.EGSRegularization.PSSRepositories.Contracts;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Parkway.CBS.POSSAP.EGSRegularization.Implementations.RecurringInvoice
{
    public class PSSRegularizationRecurringInvoiceService
    {
        private static readonly ILogger log = new Log4netLogger();

        public IUoW UoW { get; set; }

        public IProcessComp _processCompo { get; set; }

        public IPSSRegularizationUnknownOfficerRecurringInvoiceSettingsDAOManager _recurringInvoiceSettingsDAOManager { get; set; }

        private void SetRecurringInvoiceSettingsDAOManager()
        {
            if (_recurringInvoiceSettingsDAOManager == null) { _recurringInvoiceSettingsDAOManager = new PSSRegularizationUnknownOfficerRecurringInvoiceSettingsDAOManager(UoW); }
        }


        protected void SetUnitofWork(string tenantName)
        {
            if (UoW == null)
            {
                UoW = new UoW(tenantName.Replace(" ", "") + "_SessionFactory", "PSSSettlementJob");
            }
        }

        /// <summary>
        /// Start invoice processing
        /// </summary>
        /// <param name="tenantName"></param>
        /// <returns></returns>
        public void BeginInvoiceProcessing(string tenantName)
        {
            log.Info($"About to start POSSAP regularization recurring invoice processing");
            try
            {
                SetUnitofWork(tenantName);
                SetRecurringInvoiceSettingsDAOManager();

                //get recurring invoice settings with next schedule date of today
                DateTime today = DateTime.Now.Date;
                int recordCount = _recurringInvoiceSettingsDAOManager.IntCount(x => x.NextInvoiceGenerationDate == today);

                log.Info($"About to start POSSAP regularization invoice processing for today {today:dd/MM/yyyy}");
                if (recordCount < 1)
                {
                    log.Info($"No active POSSAP regularization invoice to be processed for today {today:dd/MM/yyyy}");
                    return;
                }

                string chunkSizeStringValue = ConfigurationManager.AppSettings["ChunkSize"];
                int chunkSize = 100;

                if (!string.IsNullOrEmpty(chunkSizeStringValue))
                {
                    int.TryParse(chunkSizeStringValue, out chunkSize);
                }
                //get the pages
                int pages = Util.Pages(chunkSize, recordCount);
                int stopper = 0;
                int skip = 0;

                IEnumerable<PSSRegularizationUnknownOfficerRecurringInvoiceSettingsDTO> recurringInvoiceSettings = new List<PSSRegularizationUnknownOfficerRecurringInvoiceSettingsDTO>();
                
                if(recordCount > 0)
                {
                    StartHangFireService();
                    while (stopper < pages)
                    {
                        //get records by chunksize
                        recurringInvoiceSettings = _recurringInvoiceSettingsDAOManager.GetBatchRecurringInvoiceSettings(chunkSize, skip, today);

                        foreach (PSSRegularizationUnknownOfficerRecurringInvoiceSettingsDTO recurringInvoiceDTO in recurringInvoiceSettings)
                        {
                            log.Info($"About to queue recurring invoice for request Id {recurringInvoiceDTO.RequestId} on Hangfire");
                            BackgroundJob.Enqueue(() => new DoInvoiceGeneration().GenerateInvoice(tenantName, recurringInvoiceDTO));
                            log.Info($"Recurring invoice for request Id {recurringInvoiceDTO.RequestId} queued on Hangfire");
                        }

                        recurringInvoiceSettings = Enumerable.Empty<PSSRegularizationUnknownOfficerRecurringInvoiceSettingsDTO>();
                        skip += chunkSize;
                        stopper++;
                    }
                }
            }
            catch (Exception exception)
            {
                log.Error($"Error processing POSSAP regularization recurring invoice");
                log.Error(exception.Message, exception);
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                _processCompo = null;
            }
        }


        /// <summary>
        /// Get process composition instance
        /// </summary>
        private void StartHangFireService()
        {
            if (_processCompo == null) { _processCompo = new ProcessComp(); }
            _processCompo.StartHangFireService();
        }

    }
}
