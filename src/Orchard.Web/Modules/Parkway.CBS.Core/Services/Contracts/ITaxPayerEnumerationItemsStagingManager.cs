using Orchard;
using Parkway.CBS.Core.Models;
using System.Collections.Generic;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface ITaxPayerEnumerationItemsStagingManager<TaxPayerEnumerationItemsStaging> : IDependency, IBaseManager<TaxPayerEnumerationItemsStaging>
    {
        /// <summary>
        /// Saves enumeration line items for enumeration batch with specified id.
        /// </summary>
        /// <param name="items">List<TaxPayerEnumerationLine></param>
        /// <param name="enumerationBatchId"></param>
        void SaveRecords(IEnumerable<dynamic> items, long enumerationBatchId);
    }
}
