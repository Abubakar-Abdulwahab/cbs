using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.OSGOF.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.OSGOF.Admin.Services.Contracts
{
    public interface ICellSiteClientPaymentBatchManager<CellSiteClientPaymentBatch> : IDependency, IBaseManager<CellSiteClientPaymentBatch>
    {
        /// <summary>
        /// Get the batch record with this Id
        /// </summary>
        /// <param name="recordId"></param>
        /// <returns>OSGOFBatchRecord</returns>
        CellSiteClientPaymentBatch GetRecord(long id);
    }
}
