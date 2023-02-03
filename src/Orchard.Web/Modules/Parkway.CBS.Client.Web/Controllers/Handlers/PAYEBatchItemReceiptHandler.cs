using Newtonsoft.Json;
using Orchard.Logging;
using Parkway.CBS.Client.Web.Controllers.Handlers.Contracts;
using Parkway.CBS.Core.DataFilters.PAYEBatchItemReceiptReport.Contracts;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Module.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Client.Web.Controllers.Handlers
{
    public class PAYEBatchItemReceiptHandler : IPAYEBatchItemReceiptHandler
    {
        private readonly ICoreCollectionService _coreCollectionService;
        private readonly IPAYEBatchItemReceiptFilter _payeReceiptFilter;
        private readonly ICommonHandler _commonHandler;
        public ILogger Logger { get; set; }

        public PAYEBatchItemReceiptHandler(ICoreCollectionService coreCollectionService, IPAYEBatchItemReceiptFilter payeReceiptFilter, ICommonHandler commonHandler)
        {
            Logger = NullLogger.Instance;
            _coreCollectionService = coreCollectionService;
            _payeReceiptFilter = payeReceiptFilter;
            _commonHandler = commonHandler;
        }

        public APIResponse GetPagedBatchItemReceiptData(string token, int page)
        {
            try
            {
                Logger.Information("Decrypting receipt token");
                var decryptedValue = Util.LetsDecrypt(token, System.Configuration.ConfigurationManager.AppSettings["EncryptionSecret"]);
                var tokenModel = JsonConvert.DeserializeObject<ReceiptDataModel>(decryptedValue);
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

                PAYEReceiptSearchParams searchData = new PAYEReceiptSearchParams
                {
                    ReceiptNumber = tokenModel.ReceiptNumber,
                    TaxEntityId = tokenModel.TaxEntityId,
                    Skip = skip,
                    Take = tokenModel.ChunkSize,
                    IsEmployer = tokenModel.IsEmployer,
                    EndDate = endDate,
                    StartDate = startDate
                };

                var receiptObj = _payeReceiptFilter.GetReceiptViewModel(searchData);
                var dataSize = ((IEnumerable<ReportStatsVM>)receiptObj.Aggregate).First().TotalRecordCount;
                int pages = Util.Pages(tokenModel.ChunkSize, dataSize);

                return new APIResponse { ResponseObject = new ReceiptsVM { DataSize = pages, ReceiptItems = (IEnumerable<PAYEReceiptItems>)receiptObj.ReceiptRecords, ReceiptNumber = tokenModel.ReceiptNumber } };
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Error getting paged receipt data"));
            }
            return new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().ToString() };
        }

        /// <summary>
        /// Get receipts
        /// </summary>
        public ReceiptsVM GetPAYEBatchItemReceipts(PAYEReceiptSearchParams receiptSearchParams)
        {
            var receiptObj = _payeReceiptFilter.GetReceiptViewModel(receiptSearchParams);
            var dataSize = ((IEnumerable<ReportStatsVM>)receiptObj.Aggregate).First().TotalRecordCount;
            int pages = Util.Pages(receiptSearchParams.Take, dataSize);

            return new ReceiptsVM
            {
                ReceiptNumber = receiptSearchParams.ReceiptNumber,
                HeaderObj = _commonHandler.GetHeaderObj(),
                ReceiptItems = (IEnumerable<PAYEReceiptItems>)receiptObj.ReceiptRecords,
                DataSize = pages,
                DateFilter = string.Format("{0} - {1}", receiptSearchParams.StartDate.ToString("dd'/'MM'/'yyyy"), receiptSearchParams.EndDate.ToString("dd'/'MM'/'yyyy")),
                Token = Util.LetsEncrypt(JsonConvert.SerializeObject(new ReceiptDataModel { DateFilter = string.Format("{0} - {1}", receiptSearchParams.StartDate.ToString("dd'/'MM'/'yyyy"), receiptSearchParams.EndDate.ToString("dd'/'MM'/'yyyy")), ChunkSize = 10, Page = 1, ReceiptNumber = receiptSearchParams.ReceiptNumber, TaxEntityId = receiptSearchParams.TaxEntityId, IsEmployer = receiptSearchParams.IsEmployer }), System.Configuration.ConfigurationManager.AppSettings["EncryptionSecret"])
            };
        }

    }
}