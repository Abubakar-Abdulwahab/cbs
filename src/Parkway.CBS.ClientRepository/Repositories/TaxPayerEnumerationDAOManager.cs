using System;
using System.Linq;
using NHibernate.Linq;
using Parkway.CBS.Core.Models;
using Parkway.CBS.ClientRepository.Repositories.Contracts;


namespace Parkway.CBS.ClientRepository.Repositories
{
    public class TaxPayerEnumerationDAOManager : Repository<TaxPayerEnumeration>, ITaxPayerEnumerationDAOManager
    {
        public TaxPayerEnumerationDAOManager(IUoW uow) : base(uow)
        {

        }

        /// <summary>
        /// Gets processing stage of enumeration batch with specified id.
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns></returns>
        public Core.Models.Enums.TaxPayerEnumerationProcessingStages CheckTaxPayerEnumerationBatchStatus(long batchId)
        {
            try
            {
                return _uow.Session.Query<TaxPayerEnumeration>().Where(x => x.Id == batchId).Select(x => (Core.Models.Enums.TaxPayerEnumerationProcessingStages)x.ProcessingStage).SingleOrDefault();
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Update processing stage for enumeration with specified batch id using the specified processing stage.
        /// </summary>
        /// <param name="processingStage"></param>
        /// <param name="batchId"></param>
        public void UpdateTaxPayerEnumerationBatchStatus(Core.Models.Enums.TaxPayerEnumerationProcessingStages processingStage, long batchId)
        {
            try
            {
                var queryText = $"UPDATE Parkway_CBS_Core_TaxPayerEnumeration SET ProcessingStage = :processingStage, IsActive = :active WHERE Id = :batch_Id";
                var query = _uow.Session.CreateSQLQuery(queryText);
                bool active = (processingStage == Core.Models.Enums.TaxPayerEnumerationProcessingStages.Completed) ? true : false;
                query.SetParameter("batch_Id", batchId);
                query.SetParameter("processingStage", (int)processingStage);
                query.SetParameter("active", active);
                query.ExecuteUpdate();
            }
            catch (Exception) { throw; }
        }
    }
}
