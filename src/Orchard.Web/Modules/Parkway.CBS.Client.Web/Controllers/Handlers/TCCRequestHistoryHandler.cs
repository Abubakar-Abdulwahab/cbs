using Newtonsoft.Json;
using Orchard;
using Orchard.Logging;
using Parkway.CBS.Client.Web.Controllers.Handlers.Contracts;
using Parkway.CBS.Client.Web.ViewModels;
using Parkway.CBS.Core.DataFilters.TCCReport.Contracts;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Module.Web.Controllers.CommonHandlers.HelperHandlers.Contracts;
using Parkway.CBS.Module.Web.Controllers.Handlers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Parkway.CBS.Client.Web.Controllers.Handlers
{
    public class TCCRequestHistoryHandler : CommonBaseHandler, ITCCRequestHistoryHandler
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IAdminSettingManager<ExpertSystemSettings> _settingsRepository;
        private readonly IHandlerHelper _handlerHelper;
        private readonly Lazy<ITCCRequestReportFilter> _tccRequestReportFilter;
        private readonly Lazy<ITaxClearanceCertificateRequestManager<TaxClearanceCertificateRequest>> _tccRequestManager;

        public TCCRequestHistoryHandler(IOrchardServices orchardServices, Lazy<ITCCRequestReportFilter> tccRequestReportFilter, IAdminSettingManager<ExpertSystemSettings> settingsRepository, IHandlerHelper handlerHelper, Lazy<ITaxClearanceCertificateRequestManager<TaxClearanceCertificateRequest>> tccRequestManager) : base(orchardServices, settingsRepository, handlerHelper)
        {
            _orchardServices = orchardServices;
            _tccRequestReportFilter = tccRequestReportFilter;
            _tccRequestManager = tccRequestManager;
            _settingsRepository = settingsRepository;
            _handlerHelper = handlerHelper;
        }


        /// <summary>
        /// Gets TCC Requests
        /// </summary>
        /// <param name="searchParams">search params</param>
        /// <returns>TCCRequestHistoryVM</returns>
        public TCCRequestHistoryVM GetRequests(TCCReportSearchParams searchParams)
        {
            dynamic recordsAndAggregate = _tccRequestReportFilter.Value.GetReportViewModel(searchParams);
            IEnumerable<TCCRequestVM> records = ((IEnumerable<TCCRequestVM>)recordsAndAggregate.ReportRecords);

            return new TCCRequestHistoryVM
            {
                DateFilter = String.Format("{0} - {1}", searchParams.StartDate.ToString("dd'/'MM'/'yyyy"), searchParams.EndDate.ToString("dd'/'MM'/'yyyy")),
                ApplicantName = searchParams.ApplicantName,
                ApplicationNumber = searchParams.ApplicationNumber,
                TIN = searchParams.PayerId,
                Status = searchParams.SelectedStatus,
                Requests = (records == null || !records.Any()) ? new List<TCCRequestVM> { } : records.ToList(),
                TotalRequestRecord = (int)(((IEnumerable<ReportStatsVM>)recordsAndAggregate.Aggregate).First().TotalRecordCount),
                Token = Util.LetsEncrypt(JsonConvert.SerializeObject(new RequestHistoryDataModel { ApplicantName = searchParams.ApplicantName, StartDate = searchParams.StartDate, EndDate = searchParams.EndDate, ApplicationNumber = searchParams.ApplicationNumber, OperatorId = searchParams.TaxEntityId, TIN = searchParams.PayerId, ChunkSize = 10, Status = (int)searchParams.SelectedStatus }), System.Configuration.ConfigurationManager.AppSettings["EncryptionSecret"]),
                
            };
        }


        /// <summary>
        /// Gets Paged TCC Requests
        /// </summary>
        /// <param name="token">token</param>
        /// <param name="page">page</param>
        /// <returns>APIResponse</returns>
        public APIResponse GetPagedRequestsData(string token, int? page)
        {
            try
            {
                var tokenModel = new RequestHistoryDataModel { };
                if (!String.IsNullOrEmpty(token))
                {
                    var decryptedValue = LetsDecrypt(token, System.Configuration.ConfigurationManager.AppSettings["EncryptionSecret"]);
                    tokenModel = JsonConvert.DeserializeObject<RequestHistoryDataModel>(decryptedValue);
                }
                else { throw new Exception("Empty Token provided in GetPagedRequestsData for TCCRequestHistoryHandler");  }
                DateTime startDate = DateTime.Now.AddMonths(-3);
                DateTime endDate = DateTime.Now;

                if (tokenModel.StartDate != null && tokenModel.EndDate != null)
                {
                        startDate = tokenModel.StartDate;
                        endDate = tokenModel.EndDate;
                }

                //get the date up until the final sec
                endDate = endDate.Date.AddDays(1).AddMilliseconds(-1);

                int skip = page.HasValue ? (page.Value - 1) * tokenModel.ChunkSize : 0;

                TCCReportSearchParams searchParams = new TCCReportSearchParams
                {
                    TaxEntityId = tokenModel.OperatorId,
                    Take = tokenModel.ChunkSize,
                    Skip = skip,
                    EndDate = endDate,
                    StartDate = startDate,
                    ApplicationNumber = tokenModel.ApplicationNumber,
                    ApplicantName = tokenModel.ApplicantName,
                    PayerId = tokenModel.TIN,
                    SelectedStatus = (TCCRequestStatus)tokenModel.Status,
                };

                TCCRequestHistoryVM model = GetRequests(searchParams);

                return new APIResponse { ResponseObject = model };

            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                return new APIResponse { Error = true, ResponseObject = new { Message = ErrorLang.genericexception().ToString() } };
            }
        }
    }
}