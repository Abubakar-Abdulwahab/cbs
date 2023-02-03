using Orchard;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface IPAYEBatchItemsPagesTrackerManager<PAYEAPIBatchItemsPagesTracker> : IDependency, IBaseManager<PAYEAPIBatchItemsPagesTracker>
    {
        /// <summary>
        /// Checks if the batchIdentifier and the page number already exist
        /// </summary>
        /// <param name="batchIdentifier"></param>
        /// <param name="pageNumber"></param>
        /// <returns>Boolean (True or False)</returns>
        bool BatchItemPageExist(string batchIdentifier, int pageNumber);

        /// <summary>
        /// Returns Only the BatchItemsPagesTracker Id
        /// </summary>
        /// <param name="lambda"></param>
        /// <returns>The Id of the BatchItemsPagesTracker</returns>
        /// <exception cref="NoRecordFoundException"> Thrown when no record is found matching the query</exception>
        long GetPAYEAPIBatchItemsPagesTrackerId(Expression<Func<PAYEAPIBatchItemsPagesTracker, bool>> lambda);
    }
}