using Parkway.CBS.Core.Models;

namespace Parkway.CBS.ClientRepository.Repositories.Contracts
{
    public interface ITaxPayerEnumerationDAOManager : IRepository<TaxPayerEnumeration>
    {
        /// <summary>
        /// Gets processing stage of enumeration batch with specified id.
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns></returns>
        Core.Models.Enums.TaxPayerEnumerationProcessingStages CheckTaxPayerEnumerationBatchStatus(long batchId);

        /// <summary>
        /// Update processing stage for enumeration with specified batch id using the specified processing stage.
        /// </summary>
        /// <param name="processingStage"></param>
        /// <param name="batchId"></param>
        void UpdateTaxPayerEnumerationBatchStatus(Core.Models.Enums.TaxPayerEnumerationProcessingStages processingStage, long batchId);
    }
}
