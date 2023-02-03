using Orchard.Logging;
using Parkway.CBS.Core.Models;
using Parkway.CBS.ReferenceData.Admin.CoreServices.Contracts;
using Parkway.CBS.ReferenceData.Admin.Services.Contracts;
using Parkway.CBS.ReferenceData.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.ReferenceData.Admin.CoreServices
{
    public class CoreNagisInvoiceSummary : ICoreNagisInvoiceSummary
    {
        private readonly INagisInvoiceSummaryManager<NagisOldInvoiceSummary> _nagisOldInvoiceSummaryService;
        public ILogger Logger { get; set; }

        public CoreNagisInvoiceSummary(INagisInvoiceSummaryManager<NagisOldInvoiceSummary> nagisOldInvoiceSummaryService)
        {
            _nagisOldInvoiceSummaryService = nagisOldInvoiceSummaryService;
            Logger = NullLogger.Instance;
        }

        public NAGISInvoiceSummaryVM GetInvoiceSummaries(long NagisDataBatchId)
        {
            return _nagisOldInvoiceSummaryService.GetInvoiceSummaries(NagisDataBatchId);
        }
    }
}