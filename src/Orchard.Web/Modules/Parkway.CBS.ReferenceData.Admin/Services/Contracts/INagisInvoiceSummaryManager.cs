using Orchard;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.ReferenceData.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.ReferenceData.Admin.Services.Contracts
{
    public interface INagisInvoiceSummaryManager<NagisOldInvoiceSummary> : IDependency, IBaseManager<NagisOldInvoiceSummary>
    {
        NAGISInvoiceSummaryVM GetInvoiceSummaries(long NagisDataBatchId);
    }
}
