using Orchard;
using Parkway.CBS.ReferenceData.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.ReferenceData.Admin.CoreServices.Contracts
{
    public interface ICoreNagisInvoiceSummary : IDependency
    {
        NAGISInvoiceSummaryVM GetInvoiceSummaries(long NagisDataBatchId);
    }
}
