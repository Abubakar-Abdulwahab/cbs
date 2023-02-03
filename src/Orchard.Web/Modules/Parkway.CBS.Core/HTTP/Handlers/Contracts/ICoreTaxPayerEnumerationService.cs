using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using System.Collections.Generic;

namespace Parkway.CBS.Core.HTTP.Handlers.Contracts
{
    public interface ICoreTaxPayerEnumerationService : IDependency
    {
        /// <summary>
        /// Process enumeration items for on screen form.
        /// </summary>
        /// <param name="lines">List<TaxPayerEnumerationLine></param>
        /// <param name="enumerationBatchModel"></param>
        void ProcessItemsForOnScreen(IEnumerable<dynamic> lines, TaxPayerEnumeration enumerationBatchModel);


        /// <summary>
        /// Creates the enumeration batch record entry
        /// </summary>
        /// <param name="model"></param>
        /// <returns>TaxPayerEnumeration</returns>
        TaxPayerEnumeration SaveEnumerationBatchRecord(TaxPayerEnumeration model);

        /// <summary>
        /// Checks for the processing stage of enumeration batch with specified id.
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="taxEntityId"></param>
        /// <returns></returns>
        int CheckForEnumerationScheduleUploadCompletionStatus(long batchId, long taxEntityId);

        /// <summary>
        /// Gets enumeration line items for enumeration batch with specified id.
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="taxEntityId"></param>
        /// <returns></returns>
        TaxPayerEnumerationReportVM GetReportDetails(long batchId, long taxEntityId);

        /// <summary>
        /// Get paged enumeration line items for enumeration batch with specified id.
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="taxEntityId"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        IEnumerable<TaxPayerEnumerationLine> GetPagedEnumerationLineItems(long batchId, long taxEntityId, int skip, int take = 10);
    }
}
