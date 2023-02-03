using Orchard;
using Parkway.CBS.Core.HelperModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface IPAYEAPIRequestManager<PAYEAPIRequest> : IDependency, IBaseManager<PAYEAPIRequest>
    {
        /// <summary>
        /// Checks if the batchIdentifier already exist
        /// </summary>
        /// <param name="batchIdentifier"></param>
        /// <returns>Boolean (True or False)</returns>
        bool BatchIdentifierExist(string batchIdentifier);

        /// <summary>
        /// Get API Batch details using the batchIdentifier and expertsystemId
        /// </summary>
        /// <param name="batchIdentifier"></param>
        /// <param name="expertSystemId"></param>
        /// <returns>PAYEAPIRequestBatchDetailVM</returns>
        PAYEAPIRequestBatchDetailVM GetBatchDetails(string batchIdentifier, int expertSystemId);
    }
}