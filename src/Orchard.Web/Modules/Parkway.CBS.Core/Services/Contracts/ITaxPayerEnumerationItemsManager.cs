using Orchard;
using Parkway.CBS.Core.HelperModels;
using System.Collections.Generic;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface ITaxPayerEnumerationItemsManager<TaxPayerEnumerationItems> : IDependency, IBaseManager<TaxPayerEnumerationItems>
    {
        /// <summary>
        /// Saves enumeration line items for enumeration batch with specified id.
        /// </summary>
        /// <param name="items">List<TaxPayerEnumerationLine></param>
        /// <param name="enumerationBatchId"></param>
        void SaveRecords(IEnumerable<dynamic> items, long enumerationBatchId);

        /// <summary>
        /// Returns enumeration line items for enumeration batch with specified id for the logged in or selected tax entity.
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="taxEntityId"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        IEnumerable<TaxPayerEnumerationLine> GetLineItems(long batchId, long taxEntityId, int skip, int take);

        /// <summary>
        /// Get report aggregate
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns>IEnumerable{FileUploadReport}</returns>
        IEnumerable<FileUploadReport> GetReportAggregate(long batchId, long taxEntityId);

        /// <summary>
        /// Get report size
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns>IEnumerable{FileUploadReport}</returns>
        IEnumerable<int> GetCount(long batchId);
    }
}
