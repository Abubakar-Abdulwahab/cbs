using Parkway.CBS.ClientRepository.Repositories.ReferenceDataImpl.Contracts;
using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.ClientRepository.Repositories.ReferenceDataImpl
{
    public class ReferenceDataBatchDAOManager : Repository<ReferenceDataBatch>, IReferenceDataBatchDAOManager
    {
        public ReferenceDataBatchDAOManager(IUoW uow) : base(uow)
        {

        }

        /// <summary>
        /// Get Reference Data Batch record using batchId
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns>ReferenceDataBatch</returns>
        public ReferenceDataBatch GetBatchRecord(long batchId)
        {
            try
            {
                return _uow.Session.QueryOver<ReferenceDataBatch>().Where(x => x.Id == batchId).SingleOrDefault();
            }
            catch (Exception exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get Reference Data Batch record using GeneralBatchReference Id
        /// </summary>
        /// <param name="generalBatchReferenceId"></param>
        /// <returns>ReferenceDataBatch</returns>
        public ReferenceDataBatch GetBatchDetails(long generalBatchReferenceId)
        {
            try
            {
                return _uow.Session.QueryOver<ReferenceDataBatch>().Where(x => x.GeneralBatchReference.Id == generalBatchReferenceId).SingleOrDefault();
            }
            catch (Exception exception)
            {
                throw;
            }
        }
    }
}
