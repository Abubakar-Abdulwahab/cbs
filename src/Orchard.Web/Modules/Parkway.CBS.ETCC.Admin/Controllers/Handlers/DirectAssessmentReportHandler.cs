using Orchard.Logging;
using Parkway.CBS.Core.DataFilters.DirectAssessmentReport.Contracts;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.ETCC.Admin.Controllers.Handlers.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.ETCC.Admin.Controllers.Handlers
{
    public class DirectAssessmentReportHandler : IDirectAssessmentReportHandler
    {
        private readonly Lazy<IDirectAssessmentRequestReportFilter> _assessmentRequestReportFilter;
        private readonly IPAYEDirectAssessmentTypeManager<PAYEDirectAssessmentType> _payeDirectAssessmentType;

        public ILogger Logger { get; set; }

        public DirectAssessmentReportHandler(Lazy<IDirectAssessmentRequestReportFilter> assessmentRequestReportFilter, IPAYEDirectAssessmentTypeManager<PAYEDirectAssessmentType> payeDirectAssessmentType)
        {
            _assessmentRequestReportFilter = assessmentRequestReportFilter;
            Logger = NullLogger.Instance;
            _payeDirectAssessmentType = payeDirectAssessmentType;


        }

        /// <summary>
        /// Get model for direct assessment report view
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>PAYEDirectAssessmentReportVM</returns>
        public PAYEDirectAssessmentReportVM GetDirectAssessmentReport(DirectAssessmentSearchParams searchParams)
        {
            try
            {
                dynamic recordsAndAggregate = _assessmentRequestReportFilter.Value.GetReportViewModel(searchParams);
                IEnumerable<DirectAssessmentReportItemsVM> records = (IEnumerable<DirectAssessmentReportItemsVM>)recordsAndAggregate.ReportRecords;


                var directAssessmentTypes = _payeDirectAssessmentType.GetAll();

                return new PAYEDirectAssessmentReportVM
                {
                    DirectAssessmentTypes = directAssessmentTypes,
                    From = searchParams.StartDate.ToString("dd'/'MM'/'yyyy"),
                    End = searchParams.EndDate.ToString("dd'/'MM'/'yyyy"),
                    TIN = searchParams.TIN,
                    InvoiceNo = searchParams.InvoiceNumber,
                    InvoiceStatus = searchParams.InvoiceStatus,
                    DirectAssessmentType = searchParams.DirectAssessmentType.ToString(),
                    DirectAssessmentReportItems = (records == null || !records.Any()) ? new List<DirectAssessmentReportItemsVM> { } : records.ToList(),
                    DataSize = (int)((IEnumerable<ReportStatsVM>)recordsAndAggregate.Aggregate).First().TotalRecordCount,
                    TotalAmount = ((IEnumerable<ReportStatsVM>)recordsAndAggregate.Aggregate).First().TotalAmount

                };
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ex.Message);
                throw;
            }

        }
    }
}