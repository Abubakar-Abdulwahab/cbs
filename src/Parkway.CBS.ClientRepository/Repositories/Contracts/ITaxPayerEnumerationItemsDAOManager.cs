using Parkway.CBS.Core.Models;
using Parkway.CBS.TaxPayerEnumerationService.Models;
using System.Collections.Generic;

namespace Parkway.CBS.ClientRepository.Repositories.Contracts
{
    public interface ITaxPayerEnumerationItemsDAOManager : IRepository<TaxPayerEnumerationItems>
    {
        /// <summary>
        /// Save enumeration line items as a bundle
        /// </summary>
        /// <param name="lineItems"></param>
        /// <param name="batchId"></param>
        void SaveEnumerationLineItemsRecords(List<TaxPayerEnumerationLine> lineItems, long batchId);
    }
}
