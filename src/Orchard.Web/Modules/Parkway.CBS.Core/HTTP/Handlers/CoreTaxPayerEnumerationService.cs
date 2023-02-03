using Orchard.Logging;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HTTP.Handlers
{
    public class CoreTaxPayerEnumerationService : ICoreTaxPayerEnumerationService
    {
        private readonly ITaxPayerEnumerationManager<TaxPayerEnumeration> _taxPayerEnumerationRepo;
        private readonly ITaxPayerEnumerationItemsManager<TaxPayerEnumerationItems> _taxPayerEnumerationItemsRepo;
        public ILogger Logger { get; set; }
        private int baseTake = 10;
        public CoreTaxPayerEnumerationService(ITaxPayerEnumerationManager<TaxPayerEnumeration> taxPayerEnumerationRepo, ITaxPayerEnumerationItemsManager<TaxPayerEnumerationItems> taxPayerEnumerationItemsRepo)
        {
            _taxPayerEnumerationRepo = taxPayerEnumerationRepo;
            _taxPayerEnumerationItemsRepo = taxPayerEnumerationItemsRepo;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// Process enumeration items for on screen form.
        /// </summary>
        /// <param name="lines">List<TaxPayerEnumerationLine></param>
        /// <param name="enumerationBatchModel"></param>
        public void ProcessItemsForOnScreen(IEnumerable<dynamic> lines, TaxPayerEnumeration enumerationBatchModel)
        {
            try
            {
                ProcessEnumerationLines(lines, SaveEnumerationBatchRecord(enumerationBatchModel));
                
            }catch(Exception exception)
            {
                Logger.Error(exception, $"Exception processing enumeration items for on screen. Exception Message --- {exception.Message}");
                throw;
            }
        }


        /// <summary>
        /// Creates the enumeration batch record entry.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>TaxPayerEnumeration</returns>
        public TaxPayerEnumeration SaveEnumerationBatchRecord(TaxPayerEnumeration model)
        {
            try
            {
                if (!_taxPayerEnumerationRepo.Save(model))
                {
                    throw new CouldNotSaveRecord();
                }
                return model;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, $"Exception when trying to save enumeration record. Exception message -- {exception.Message}");
                _taxPayerEnumerationRepo.RollBackAllTransactions();
                throw;
            }
        }


        private void ProcessEnumerationLines(dynamic lines, TaxPayerEnumeration enumerationBatch)
        {
            SaveItems(lines, enumerationBatch.Id);
        }


        /// <summary>
        /// Saves enumeration line items.
        /// </summary>
        /// <param name="items">List<TaxPayerEnumerationLine></param>
        /// <param name="enumerationBatchId"></param>
        private void SaveItems(IEnumerable<dynamic> items, long enumerationBatchId)
        {
            _taxPayerEnumerationItemsRepo.SaveRecords(items, enumerationBatchId);
        }


        /// <summary>
        /// Checks for the processing stage of enumeration batch with specified id.
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="taxEntityId"></param>
        /// <returns></returns>
        public int CheckForEnumerationScheduleUploadCompletionStatus(long batchId, long taxEntityId)
        {
            return _taxPayerEnumerationRepo.Get(x => x.Id == batchId && x.Employer == new TaxEntity { Id = taxEntityId }).ProcessingStage;
        }


        /// <summary>
        /// Gets enumeration line items for enumeration batch with specified id.
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="taxEntityId"></param>
        /// <returns></returns>
        public TaxPayerEnumerationReportVM GetReportDetails(long batchId, long taxEntityId)
        {
            IEnumerable<TaxPayerEnumerationLine> lineItems = _taxPayerEnumerationItemsRepo.GetLineItems(batchId, taxEntityId, 0, baseTake);
            IEnumerable<FileUploadReport> aggr = _taxPayerEnumerationItemsRepo.GetReportAggregate(batchId, taxEntityId);
            IEnumerable<int> count = _taxPayerEnumerationItemsRepo.GetCount(batchId);
            if (aggr.First() == null || (lineItems == null || !lineItems.Any())) { throw new NoRecordFoundException { }; }
            aggr.First().NumberOfRecords = count.First();
            int pages = Util.Pages(baseTake, aggr.First().NumberOfRecords);
            return new TaxPayerEnumerationReportVM
            {
                PageSize = pages,
                LineItems = lineItems.ToList(),
                EnumerationItemsExcelReport = aggr.First()
            };
        }


        /// <summary>
        /// Get paged enumeration line items for enumeration batch with specified id.
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="taxEntityId"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public IEnumerable<TaxPayerEnumerationLine> GetPagedEnumerationLineItems(long batchId, long taxEntityId, int skip, int take = 10)
        {
            return _taxPayerEnumerationItemsRepo.GetLineItems(batchId, taxEntityId, skip, take).ToList();
        }

    }
}