using Orchard.Logging;
using Parkway.CBS.Client.Web.Controllers.Handlers.Contracts;
using Parkway.CBS.Core.DataFilters.PAYEBatchItems.Contracts;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Client.Web.Controllers.Handlers
{
    public class PAYEBatchItemSearchHandler : IPAYEBatchItemSearchHandler
    {
        ILogger Logger { get; set; }
        private readonly IPAYEBatchItemsFilter _payeBatchItemsFilter;
        public PAYEBatchItemSearchHandler(IPAYEBatchItemsFilter payeBatchItemsFilter)
        {
            Logger = NullLogger.Instance;
            _payeBatchItemsFilter = payeBatchItemsFilter;
        }

        /// <summary>
        /// Get PAYE Batch items list vm
        /// </summary>
        /// <param name="batchRef"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public PAYEBatchItemsListVM GetPAYEBatchItemsListVM(string batchRef, int page)
        {
            try
            {
                PAYEBatchItemSearchParams searchParams = new PAYEBatchItemSearchParams
                {
                    BatchRef = batchRef,
                    Skip = page == 1 ? 0 : (10 * page) - 10,
                    Take = 10,
                    PageData = true,
                };

                var batchItems = _payeBatchItemsFilter.GetBatchItems(searchParams);
                var dataSize = ((IEnumerable<ReportStatsVM>)batchItems.Aggregate).First().TotalRecordCount;
                int pages = Util.Pages(searchParams.Take, dataSize);
                return new PAYEBatchItemsListVM { BatchRef = batchRef, BatchItems = batchItems.BatchItemRecords, DataSize = pages };
            }
            catch (Exception exception) { Logger.Error(exception, exception.Message);  throw; }
        }
    }
}