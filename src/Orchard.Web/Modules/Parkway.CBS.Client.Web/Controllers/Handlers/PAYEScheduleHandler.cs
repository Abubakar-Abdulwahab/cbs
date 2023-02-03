using Newtonsoft.Json;
using Orchard.Logging;
using Parkway.CBS.Client.Web.Controllers.Handlers.Contracts;
using Parkway.CBS.Core.DataFilters.PAYEBatchRecord.Contracts;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Parkway.CBS.Client.Web.Controllers.Handlers
{
    public class PAYEScheduleHandler : IPAYEScheduleHandler
    {
        private readonly IPAYEBatchRecordFilter _payeBatchRecordFilter;
        private readonly ICommonHandler _commonHandler;
        public ILogger Logger { get; set; }
        private readonly IPAYEPaymentUtilizationManager<PAYEPaymentUtilization> _payeUtilizationRepo;

        public PAYEScheduleHandler(IPAYEBatchRecordFilter payeBatchRecordFilter, ICommonHandler commonHandler, IPAYEPaymentUtilizationManager<PAYEPaymentUtilization> payeUtilizationRepo)
        {
            Logger = NullLogger.Instance;
            _payeBatchRecordFilter = payeBatchRecordFilter;
            _commonHandler = commonHandler;
            _payeUtilizationRepo = payeUtilizationRepo;
        }

        /// <summary>
        /// Get paged batch records
        /// </summary>
        /// <param name="token"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public APIResponse GetPagedBatchRecordsData(string token, int page)
        {
            try
            {
                Logger.Information("Decrypting batch record token");
                var decryptedValue = Util.LetsDecrypt(token, System.Configuration.ConfigurationManager.AppSettings["EncryptionSecret"]);
                var tokenModel = JsonConvert.DeserializeObject<PAYEBatchRecordDataModel>(decryptedValue);
                var skip = page == 1 ? 0 : (tokenModel.ChunkSize * page) - tokenModel.ChunkSize;

                DateTime startDate = DateTime.Now.AddMonths(-3);
                DateTime endDate = DateTime.Now;
                if (!string.IsNullOrEmpty(tokenModel.DateFilter))
                {
                    var dateFilterSplit = tokenModel.DateFilter.Split(new[] { '-' }, 2);
                    if (dateFilterSplit.Length == 2)
                    {
                        try
                        {
                            startDate = DateTime.ParseExact(dateFilterSplit[0].Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                            endDate = DateTime.ParseExact(dateFilterSplit[1].Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture).AddDays(1).AddMilliseconds(-1);
                        }
                        catch (Exception)
                        {
                            startDate = DateTime.Now.AddMonths(-3);
                            endDate = DateTime.Now;
                        }
                    }
                }

                PAYEBatchRecordSearchParams searchData = new PAYEBatchRecordSearchParams
                {
                    BatchRef = tokenModel.BatchRef,
                    TaxEntityId = tokenModel.TaxEntityId,
                    Skip = skip,
                    Take = tokenModel.ChunkSize,
                    EndDate = endDate,
                    StartDate = startDate
                };

                var batchRecordsObj = _payeBatchRecordFilter.GetBatchRecordsViewModel(searchData);
                var dataSize = ((IEnumerable<ReportStatsVM>)batchRecordsObj.Aggregate).First().TotalRecordCount;
                int pages = Util.Pages(tokenModel.ChunkSize, dataSize);

                return new APIResponse { ResponseObject = new PAYEScheduleListVM { DataSize = pages, BatchRecords = (IEnumerable<PAYEBatchRecordVM>)batchRecordsObj.BatchRecords, BatchRef = tokenModel.BatchRef } };
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Error getting paged batch record data"));
            }
            return new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().ToString() };
        }

        /// <summary>
        /// Get batch records
        /// </summary>
        public PAYEScheduleListVM GetPAYEBatchRecords(PAYEBatchRecordSearchParams searchParams)
        {
            var batchRecordsObj = _payeBatchRecordFilter.GetBatchRecordsViewModel(searchParams);
            var dataSize = ((IEnumerable<ReportStatsVM>)batchRecordsObj.Aggregate).First().TotalRecordCount;
            int pages = Util.Pages(searchParams.Take, dataSize);

            return new PAYEScheduleListVM
            {
                BatchRef = searchParams.BatchRef,
                HeaderObj = _commonHandler.GetHeaderObj(),
                BatchRecords = (IEnumerable<PAYEBatchRecordVM>)batchRecordsObj.BatchRecords,
                DataSize = pages,
                DateFilter = string.Format("{0} - {1}", searchParams.StartDate.ToString("dd'/'MM'/'yyyy"), searchParams.EndDate.ToString("dd'/'MM'/'yyyy")),
                Token = Util.LetsEncrypt(JsonConvert.SerializeObject(new PAYEBatchRecordDataModel { DateFilter = string.Format("{0} - {1}", searchParams.StartDate.ToString("dd'/'MM'/'yyyy"), searchParams.EndDate.ToString("dd'/'MM'/'yyyy")), ChunkSize = 10, Page = 1, BatchRef = searchParams.BatchRef, TaxEntityId = searchParams.TaxEntityId}), System.Configuration.ConfigurationManager.AppSettings["EncryptionSecret"])
            };
        }

        /// <summary>
        /// Get utilized receipts for schedule with specified batch ref
        /// </summary>
        /// <param name="batchRef"></param>
        /// <returns></returns>
        public UtilizedReceiptsVM GetUtilizedReceipts(string batchRef)
        {
            return new UtilizedReceiptsVM
            {
                HeaderObj = _commonHandler.GetHeaderObj(),
                BatchRef = batchRef,
                UtilizedReceipts = _payeUtilizationRepo.GetUtilizedReceiptsForBatchRecord(batchRef),
            };
        }

    }
}