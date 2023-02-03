using Orchard;
using Parkway.CBS.Core.HelperModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Module.API.Controllers.Handlers.Contracts
{
    public interface IAPIPAYEHandler : IDependency
    {
        /// <summary>
        /// Processes the initialize batch request and returns an API response
        /// </summary>
        /// <param name="model"></param>
        /// <param name="headerParams"></param>
        /// <returns>APIResponse</returns>
        APIResponse ProcessInitializeRequest(PAYEIntializeBatchRequestModel model, dynamic headerParams);

        /// <summary>
        /// Processes the add batch items request and returns an API response
        /// </summary>
        /// <param name="model"></param>
        /// <param name="headerParams"></param>
        /// <returns>APIResponse</returns>

        APIResponse ProcessAddBatchItemsRequest(PAYEAddBatchItemsRequestModel model, dynamic headerParams);
    }
}