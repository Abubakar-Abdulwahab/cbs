using Orchard;
using Parkway.CBS.Core.HelperModels;
using System.Collections.Generic;

namespace Parkway.CBS.Client.Web.Controllers.Handlers.Contracts
{
    public interface IPAYEBatchItemSearchHandler : IDependency
    {
        /// <summary>
        /// Get PAYE Batch items list vm
        /// </summary>
        /// <param name="batchRef"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        PAYEBatchItemsListVM GetPAYEBatchItemsListVM(string batchRef, int page);
    }
}
