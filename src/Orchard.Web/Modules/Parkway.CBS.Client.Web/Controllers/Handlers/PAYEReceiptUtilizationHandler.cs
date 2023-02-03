using Newtonsoft.Json;
using Orchard.Logging;
using Parkway.CBS.Client.Web.Controllers.Handlers.Contracts;
using Parkway.CBS.Core.DataFilters.PAYEReceiptUtilizationReport.Contracts;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Parkway.CBS.Client.Web.Controllers.Handlers
{
    public class PAYEReceiptUtilizationHandler : IPAYEReceiptUtilizationHandler
    {
        private readonly IPAYEReceiptUtilizationFilter _payeReceiptUtilizationFilter;
        private readonly ICommonHandler _commonHandler;
        public ILogger Logger { get; set; }

        public PAYEReceiptUtilizationHandler(IPAYEReceiptUtilizationFilter payeReceiptUtilizationFilter, ICommonHandler commonHandler)
        {
            Logger = NullLogger.Instance;
            _payeReceiptUtilizationFilter = payeReceiptUtilizationFilter;
            _commonHandler = commonHandler;
        }

        /// <summary>
        /// Get PAYE receipts list
        /// </summary>
        public PAYEReceiptObj GetPAYEReceipts(PAYEReceiptSearchParams receiptSearchParams)
        {
            var receiptObj = _payeReceiptUtilizationFilter.GetReceiptViewModel(receiptSearchParams);
            var dataSize = ((IEnumerable<ReportStatsVM>)receiptObj.Aggregate).First().TotalRecordCount;
            int pages = Util.Pages(receiptSearchParams.Take, dataSize);

            return new PAYEReceiptObj
            {
                ReceiptNumber = receiptSearchParams.ReceiptNumber,
                HeaderObj = _commonHandler.GetHeaderObj(),
                ReceiptItems = (IEnumerable<PAYEReceiptVM>)receiptObj.ReceiptRecords,
                DataSize = pages,
                DateFilter = string.Format("{0} - {1}", receiptSearchParams.StartDate.ToString("dd'/'MM'/'yyyy"), receiptSearchParams.EndDate.ToString("dd'/'MM'/'yyyy")),
                Token = Util.LetsEncrypt(JsonConvert.SerializeObject(new ReceiptDataModel { DateFilter = string.Format("{0} - {1}", receiptSearchParams.StartDate.ToString("dd'/'MM'/'yyyy"), receiptSearchParams.EndDate.ToString("dd'/'MM'/'yyyy")), ChunkSize = 10, Page = 1, ReceiptNumber = receiptSearchParams.ReceiptNumber, TaxEntityId = receiptSearchParams.TaxEntityId, IsEmployer = receiptSearchParams.IsEmployer }), System.Configuration.ConfigurationManager.AppSettings["EncryptionSecret"])
            };
        }


        /// <summary>
        /// Gets receipt utilizations report for a specified receipt number
        /// </summary>
        /// <param name="receiptNumber"></param>
        /// <returns></returns>
        public PAYEReceiptUtilizationReportObj GetUtilizationsReport(string receiptNumber)
        {
            try
            {
                var receiptUtilizationsObj = _payeReceiptUtilizationFilter.GetReceiptUtilizations(receiptNumber);
                return new PAYEReceiptUtilizationReportObj
                {
                    HeaderObj = _commonHandler.GetHeaderObj(),
                    ReceiptUtilizationItems = receiptUtilizationsObj,
                };
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

        /// <summary>
        /// Get paginated PAYE receipts list
        /// </summary>
        /// <param name="token"></param>
        /// <param name="page"></param>
        /// <returns>APIResponse</returns>
        public APIResponse GetPagedPAYEReceiptData(string token, int page)
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
                    EndDate = endDate,
                    StartDate = startDate
                };

                var receiptObj = _payeReceiptUtilizationFilter.GetReceiptViewModel(searchData);
                var dataSize = ((IEnumerable<ReportStatsVM>)receiptObj.Aggregate).First().TotalRecordCount;
                int pages = Util.Pages(tokenModel.ChunkSize, dataSize);

                return new APIResponse { ResponseObject = new PAYEReceiptObj { DataSize = pages, ReceiptItems = (IEnumerable<PAYEReceiptVM>)receiptObj.ReceiptRecords, ReceiptNumber = tokenModel.ReceiptNumber } };
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Error getting paged receipt data"));
            }
            return new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().ToString() };
        }

    }
}