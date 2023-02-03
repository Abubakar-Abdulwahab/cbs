using Orchard;
using Parkway.CBS.Police.Admin.VM;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts
{
    public interface IPSSCollectionReportHandler : IDependency
    {
        /// <summary>
        /// Get police service report details
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        CollectionReportVM GetVMForRequestReport(PSSCollectionSearchParams searchParams);
    }
}
