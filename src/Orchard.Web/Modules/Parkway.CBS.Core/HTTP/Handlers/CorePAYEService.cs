using Orchard.Logging;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Payee;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Payee;
using Parkway.CBS.Payee.PayeeAdapters;
using Parkway.CBS.Payee.PayeeAdapters.Contracts;
using Parkway.CBS.Payee.PayeeAdapters.ETCC;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Core.HTTP.Handlers
{
    public class CorePAYEService : ICorePAYEService
    {

        private readonly IPAYEBatchItemsStagingManager<PAYEBatchItemsStaging> _payeItemsStagingrepo;
        IPAYEBatchRecordStagingManager<PAYEBatchRecordStaging> _payeStagingrepo;
        private readonly ITenantStateSettings<TenantCBSSettings> _tenantStateSettings;

        public readonly IPayeeAssessmentConfiguration _payeeConfig;

        public ILogger Logger { get; set; }
        private int baseTake = 10;


        public CorePAYEService(IPAYEBatchRecordStagingManager<PAYEBatchRecordStaging> payeStagingrepo, IPAYEBatchItemsStagingManager<PAYEBatchItemsStaging> payeItemsStagingrepo, IPayeeAssessmentConfiguration payeeConfig, ITenantStateSettings<TenantCBSSettings> tenantStateSettings)
        {
            _payeStagingrepo = payeStagingrepo;
            _payeItemsStagingrepo = payeItemsStagingrepo;
            _tenantStateSettings = tenantStateSettings;
            Logger = NullLogger.Instance;
            _payeeConfig = payeeConfig;
        }


        /// <summary>
        /// Process and save staging data for paye items
        /// </summary>
        /// <param name="savedBatchRecord"></param>
        /// <param name="payelines"></param>
        /// <param name="adapter"></param>
        /// <param name="entity"></param>
        public void ProcessPayeItemsForStagingInput(PAYEBatchRecordStaging record, ICollection<PAYEAssessmentLine> payelines, AssessmentInterface adapter)
        {
            try
            {
                SavePAYEs(payelines.ToList(), record.Id);

            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Could not save onscreen paye details" + exception.Message);
                record.ErrorOccurred = true;
                record.ErrorMessage = "Error processing your request";
                throw exception;
            }
        }

        private IPayeeAdapter GetAdapterImpl(AssessmentInterface adapter)
        {
            Logger.Information(string.Format("Adapter gotten {0}. Getting implementation now", adapter.ClassName));
            return _payeeConfig.GetAdapterImplementation(adapter);
        }

        /// <summary>
        /// Process PAYE items from file
        /// </summary>
        /// <param name="savedBatchRecord"></param>
        /// <param name="adapter"></param>
        public void ProcessPayeItemsForFileUploadStagingInput(PAYEBatchRecordStaging record, AssessmentInterface adapter)
        {
            try
            {
                //process file
                IPayeeAdapter adpt = GetAdapterImpl(adapter);
                Logger.Information(string.Format("Impl gotten for adatper {0} {1}. Getting the list of payee model", adapter.ClassName, adapter.Value));
                GetPayeResponse result = adpt.GetPayeeModels(record.FilePath, string.Empty, string.Empty);

                if (result.HeaderValidateObject.Error)
                { record.ErrorOccurred = true; record.ErrorMessage = result.HeaderValidateObject.ErrorMessage; return; }

                SavePAYEForFileLineItems(GetBreakDown(result.Payes, adpt).Payees, record.Id);
                record.TotalNoOfRowsProcessed = result.Payes.Count;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Could not save onscreen paye details" + exception.Message);
                record.ErrorOccurred = true;
                record.ErrorMessage = "Error processing your request";
                throw exception;
            }
        }

        /// <summary>
        /// Save line items for the payes in the breakdown
        /// </summary>
        /// <param name="payees"></param>
        /// <param name="batchStagingId"></param>
        private void SavePAYEForFileLineItems(List<PayeeAssessmentLineRecordModel> payees, long batchStagingId)
        {
            _payeItemsStagingrepo.SavePAYELineItemsRecords(payees, batchStagingId);
        }

        /// <summary>
        /// Get tax breakdown for each items
        /// </summary>
        /// <param name="payes"></param>
        /// <returns>PayeeAmountAndBreakDown</returns>
        private PayeeAmountAndBreakDown GetBreakDown(List<PayeeAssessmentLineRecordModel> payes, IPayeeAdapter adapter)
        {
            return adapter.GetRequestBreakDown(payes);
        }


        /// <summary>
        /// Save PAYE line items
        /// </summary>
        /// <param name="payees"></param>
        /// <param name="batchStagingId"></param>
        private void SavePAYEs(List<PAYEAssessmentLine> payees, Int64 batchStagingId)
        {
            _payeItemsStagingrepo.SavePAYEAssessmentLineItems(payees, batchStagingId);
        }


        /// <summary>
        /// Get direct assessment adapter
        /// </summary>
        /// <param name="adapterValue"></param>
        /// <returns>AssessmentInterface | null</returns>
        public AssessmentInterface GetDirectAssessmentAdapter(string adapterValue)
        {
            Logger.Information("Getiing tenant object. Calling db");
            TenantCBSSettings tenant = _tenantStateSettings.GetCollection(x => x.Id != 0).FirstOrDefault();
            if (tenant == null) { throw new TenantNotFoundException("Tenant setting not found"); }
            AssessmentInterface assessmentInterface = _payeeConfig.GetAssessmentType(adapterValue, tenant.Identifier);
            assessmentInterface.StateName = tenant.StateName;
            return assessmentInterface;
        }


        /// <summary>
        /// Process PAYE assessment 
        /// <para>Set the records with the tax entity Id gotten from the staging items table 
        /// and set the has errors value to true if no user was found with the payer Id</para>
        /// </summary>
        /// <param name="objValue"></param>
        public void ProcessPAYEAssessment(FileProcessModel objValue)
        {
            try
            {
                //get the tax entity Id
                _payeItemsStagingrepo.PopulateTaxEntityId(objValue.Id);
                //update has errors for items that dont have a tax entity Id and the has errors flag is false
                _payeItemsStagingrepo.SetHasErrorsForNullTaxEntity(objValue.Id);
                //get the entry count for 
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Error in batct processing {0}", exception.Message));
                _payeItemsStagingrepo.RollBackAllTransactions();
                throw;
            }
        }


        /// <summary>
        /// Check to see if the staging batch record has completed processing
        /// </summary>
        /// <param name="objValue"></param>
        /// <returns>APIResponse</returns>
        public APIResponse CheckForCompletionPercentage(FileProcessModel objValue)
        {
            //check that the record exists
            PAYEBatchRecordStagingVM record = _payeStagingrepo.GetVM(objValue.Id);
            if (record == null)
            {
                return new APIResponse { Error = true, ResponseObject = ErrorLang.norecord404().ToString() };
            }

            if (record.ErrorOccurred)
            {
                return new APIResponse { Error = true, ResponseObject = record.ErrorMessage };
            }

            return new APIResponse { ResponseObject = record.PercentageProgress };
        }



        public DirectAssessmentReportVM GetReportDetails(long batchId, long taxEntityId)
        {
            IEnumerable<PayeeReturnModelVM> listOfPayes = GetPagedDataForBatchItemsStaging(batchId, taxEntityId, 0, baseTake);
            IEnumerable<FileUploadReport> aggr = _payeItemsStagingrepo.GetReportAggregate(batchId, taxEntityId);
            IEnumerable<int> count = _payeItemsStagingrepo.GetCount(batchId);
            if (aggr.First() == null || (listOfPayes == null || !listOfPayes.Any())) { throw new NoRecordFoundException { }; }
            aggr.First().NumberOfRecords = count.First();
            int pages = Util.Pages(baseTake, aggr.First().NumberOfRecords);
            return new DirectAssessmentReportVM
            {
                Amount = string.Format("{0:n}", aggr.First().TotalAmountToBePaid),
                PageSize = pages,
                Payees = listOfPayes.ToList(),
                PayeeExcelReport = aggr.First()
            };
        }


        public IEnumerable<PayeeReturnModelVM> GetPagedDataForBatchItemsStaging(long batchId, long taxEntityId, int skip, int take)
        {
            return _payeItemsStagingrepo.GetListOfPayes(batchId, taxEntityId, skip, take);
        }


        /// <summary>
        /// Get batch amount deets
        /// </summary>
        /// <param name="id"></param>
        /// <param name="taxEntityId"></param>
        /// <returns>decimal</returns>
        /// <exception cref="NoRecordFoundException"></exception>
        public decimal GetTotalAmountForValidEntriesInBatchStaging(long id, long taxEntityId)
        {
            IEnumerable<PAYEBatchRecordStaging> batchRecordF = _payeStagingrepo.GetBatchStagingId(id, taxEntityId);
            IEnumerable<FileUploadReport> stats = _payeItemsStagingrepo.GetReportAggregate(id, taxEntityId);
            if (batchRecordF.FirstOrDefault() == null) { throw new NoRecordFoundException("no record found for batch staging " + id); }
            return stats.First().TotalAmountToBePaid;
        }


    }
}