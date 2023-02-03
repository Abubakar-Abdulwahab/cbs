using Orchard;
using Parkway.CBS.Core.HelperModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Client.Web.Controllers.Handlers.Contracts
{
    public interface IPAYEScheduleHandler : IDependency
    {
        /// <summary>
        /// Get batch records
        /// </summary>
        PAYEScheduleListVM GetPAYEBatchRecords(PAYEBatchRecordSearchParams searchParams);

        /// <summary>
        /// Get paginated record of PAYE batch records
        /// </summary>
        /// <param name="token"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        APIResponse GetPagedBatchRecordsData(string token, int page);

        /// <summary>
        /// Get utilized receipts for schedule with specified batch ref
        /// </summary>
        /// <param name="batchRef"></param>
        /// <returns></returns>
        UtilizedReceiptsVM GetUtilizedReceipts(string batchRef);
    }
}
