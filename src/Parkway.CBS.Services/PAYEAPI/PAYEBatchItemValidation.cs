using Parkway.CBS.Services.PAYEAPI.Contracts;
using Hangfire;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.HangFireInterface.Configuration;
using Parkway.CBS.Services.Implementations.Contracts;
using Parkway.CBS.Services.Logger;
using Parkway.CBS.Services.Logger.Contracts;
using System;
using Parkway.CBS.ClientRepository;
using Parkway.CBS.ClientRepository.Repositories.PAYEAPI.Contracts;
using Parkway.CBS.ClientRepository.Repositories.PAYEAPI;
using System.Configuration;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Payee;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.HTTP.Handlers;
using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Payee.PayeeAdapters.ETCC;
using System.Collections.Generic;
using Parkway.CBS.Payee.PayeeAdapters.Contracts;
using Parkway.CBS.Payee.PayeeAdapters;
using Parkway.CBS.Payee;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Linq;

namespace Parkway.CBS.Services.PAYEAPI
{
    public class PAYEBatchItemValidation : IPAYEBatchItemValidation
    {
        private static readonly ILogger log = new ServicesLog4netLogger();

        public IPAYEAPIRequestDAOManager payeAPIRequestDAO { get; set; }

        public IPAYEAPIBatchItemsRefDAOManager payeBatchItemsRefDAO { get; set; }

        public IDirectAssessmentPayee directAssessmentPayee { get; set; }


        public IUoW UoW { get; set; }

        protected void SetUnitofWork(string tenantName)
        {
            if (UoW == null)
            {
                UoW = new UoW(tenantName.Replace(" ", "") + "_SessionFactory", "PSSAllowanceJob");
            }
        }

        private void SetPAYEAPIRequestDAOManager()
        {
            if (payeAPIRequestDAO == null) { payeAPIRequestDAO = new PAYEAPIRequestDAOManagerDAOManager(UoW); }
        }

        private void SetPAYEBatchItemsRefDAOManager()
        {
            if (payeBatchItemsRefDAO == null) { payeBatchItemsRefDAO = new PAYEAPIBatchItemsRefDAOManager(UoW); }
        }

        private void SetDirectAssessmentPayee()
        {
            if (directAssessmentPayee == null) { directAssessmentPayee = new DirectAssessmentPayee(); }
        }
        
        /// <summary>
        /// Start PAYE batch validation 
        /// </summary>
        /// <param name="tenantName"></param>
        /// <param name="batchIdentifier"></param>
        /// <param name="expertSystemId"></param>
        /// <param name="adapter"></param>
        public void ProcessPAYEBatchItemsValidation(string tenantName, string batchIdentifier, int expertSystemId, AssessmentInterface adapter)
        {
            log.Info($"About to queue PAYE batch items for validation using Hangfire job");
            try
            {
                var conStringNameKey = AppSettingsConfigurations.GetSettingsValue(AppSettingEnum.HangfireConnectionStringName);
                if (string.IsNullOrEmpty(conStringNameKey))
                {
                    throw new Exception("Unable to get the hangfire connection string name");
                }

                StartHangfireServer(conStringNameKey);
                BackgroundJob.Enqueue(() => ValidateBatchItems(tenantName, batchIdentifier, expertSystemId, adapter));
            }
            catch (Exception exception)
            {
                log.Error(string.Format("Error while queuing batch invoice response to the Hangfire for stage {0}", ReferenceDataProcessingStages.UpdateInvoiceStagingWithCashFlowResponse));
                log.Error(exception.Message, exception);
                throw;
            }
        }

        private void StartHangfireServer(string conStringNameKey)
        {
            //Get the connection string
            string dbConnectionString = HangFireScheduler.GetConnectionString(conStringNameKey);
            //Call the Hangfire storage
            GlobalConfiguration.Configuration.UseSqlServerStorage(dbConnectionString);
        }

        public void ValidateBatchItems(string tenantName, string batchIdentifier, int expertSystemId, AssessmentInterface adapter)
        {
            log.Info($"About to start PAYE batch items validation. Batch Identifier {batchIdentifier}");
            try
            {
                SetUnitofWork(tenantName);
                SetPAYEAPIRequestDAOManager();
                SetPAYEBatchItemsRefDAOManager();

                PAYEAPIRequest batchInfo = payeAPIRequestDAO.GetAPIRequestDetails(batchIdentifier, expertSystemId);
                if(batchInfo == null)
                {
                    throw new Exception($"Batch Identifier {batchIdentifier} not found");
                }

                CheckProcessStage(batchInfo, PAYEAPIProcessingStages.BatchItemsSaved);

                Int64 recordCount = payeBatchItemsRefDAO.Count(r => r.PAYEBatchItemsStaging.PAYEBatchRecordStaging == new PAYEBatchRecordStaging { Id = batchInfo.PAYEBatchRecordStaging.Id });
                log.Info($"About to validate {recordCount} PAYE batch items. Batch Identifier {batchIdentifier}");
                if (recordCount < 1) return;

                List<PAYEAPIBatchItemsRefVM> validationResult = new List<PAYEAPIBatchItemsRefVM> { };
                int chunkSize = batchInfo.BatchLimit;
                //get the pages
                int pages = Util.Pages(chunkSize, recordCount);
                int stopper = 0;
                int skip = 0;
                while (stopper < pages)
                {
                    List<PAYEAPIBatchItemsRefVM> payees = payeBatchItemsRefDAO.GetBatchItems(batchInfo.PAYEBatchRecordStaging.Id, chunkSize, skip);
                    List<PAYEAPIBatchItemsRefVM> payeesValidatedItems = ProcessPayeItems(batchInfo.PAYEBatchRecordStaging, payees, adapter, batchInfo.RequestedByExpertSystem.ClientSecret);                    
                    validationResult.AddRange(payeesValidatedItems.Where(x => x.HasError));
                    skip += chunkSize;
                    stopper++;

                    UoW.BeginTransaction();

                    //Save records to the db
                    payeBatchItemsRefDAO.SaveRecords(batchInfo.PAYEBatchRecordStaging.Id, validationResult, batchInfo.BatchLimit);

                    //Update taxentity
                    payeBatchItemsRefDAO.PopulateTaxEntityId(batchInfo.PAYEBatchRecordStaging.Id);

                    //Update the error for tax entity
                    payeBatchItemsRefDAO.SetHasErrorsForNullTaxEntity(batchInfo.PAYEBatchRecordStaging.Id);

                    //Update the errors after validation
                    payeBatchItemsRefDAO.UpdateErrorMessageAfterItemValidation(batchInfo.PAYEBatchRecordStaging.Id);
                    payeBatchItemsRefDAO.SetHasErrorsAfterItemValidation(batchInfo.PAYEBatchRecordStaging.Id);
                }

                batchInfo.ProcessingStage = (int)PAYEAPIProcessingStages.BatchValidated;
                UoW.Commit();
            }
            catch (Exception exception)
            {
                log.Error($"Error while validating batch items for batch identifier {batchIdentifier} for stage {PAYEAPIProcessingStages.BatchConfirmed.ToDescription()}");
                log.Error(exception.Message, exception);
                throw;
            }
        }

        private List<PAYEAPIBatchItemsRefVM> ProcessPayeItems(PAYEBatchRecordStaging record, List<PAYEAPIBatchItemsRefVM> payelines, AssessmentInterface adapter, string clientSecret)
        {
            try
            {
                //process the items
                IPayeeAdapter adpt = GetAdapterImpl(adapter);
                log.Info(string.Format("Impl gotten for adatper {0} {1}. Getting the list of payee model", adapter.ClassName, adapter.Value));
                foreach (var item in payelines)
                {
                    List<PAYEAssessmentLine> payeAssessmentLines = new List<PAYEAssessmentLine>();
                    payeAssessmentLines.Add(new PAYEAssessmentLine { PayerId = item.PayerId, Exemptions = item.Exemptions, GrossAnnualEarning = item.GrossAnnualEarning, Month = item.Month, Year = item.Year});
                    List<PayeeAssessmentLineRecordModel> payee = adpt.GetPAYEModels(payeAssessmentLines, string.Empty, adapter.StateName);
                    item.HasError = payee[0].HasError;
                    item.ErrorMessages = payee[0].ErrorMessages;

                    string macString = $"{item.PayerId}{item.IncomeTaxPerMonth.ToString("F")}{item.Month}{item.Year}";
                    if (item.Mac != Util.HMACHash256(macString, clientSecret))
                    {
                        if (item.HasError)
                        {
                            item.ErrorMessages += $" | Unable to compute Mac signature.";
                        }
                        else
                        {
                            item.HasError = true;
                            item.ErrorMessages = $"Unable to compute Mac signature.";
                        }
                    }
                }
                return payelines;
            }
            catch (Exception exception)
            {
                log.Error("Could not validate paye details" + exception.Message, exception);
                record.ErrorOccurred = true;
                record.ErrorMessage = "Error processing your request";
                throw exception;
            }
        }

        private IPayeeAdapter GetAdapterImpl(AssessmentInterface adapter)
        {
            log.Info(string.Format("Adapter gotten {0}. Getting implementation now", adapter.ClassName));
            SetDirectAssessmentPayee();
            return directAssessmentPayee.GetAdapter(adapter.ClassName);
        }

        private void CheckProcessStage(PAYEAPIRequest batchRecord, PAYEAPIProcessingStages expectedStage)
        {
            if (batchRecord != null)
            {
                //lets check what stage it is in
                //for this method the expected stage is FileValidationProcessed
                if (batchRecord.ProcessingStage != (int)expectedStage)
                { throw new InvalidOperationException($"PAYE processing has already passed this stage. Current stage {(PAYEAPIProcessingStages)batchRecord.ProcessingStage}, Expected stage {expectedStage}, Batch staging Id {batchRecord.PAYEBatchRecordStaging.Id}"); }
            }
        }

    }
}
