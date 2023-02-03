using NHibernate.Linq;
using Parkway.CBS.ClientRepository;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.POSSAP.EGSRegularization.HelperModel;
using Parkway.CBS.POSSAP.EGSRegularization.PSSRepositories.Contracts;
using System;
using System.Linq;

namespace Parkway.CBS.POSSAP.EGSRegularization.PSSRepositories
{
    public class PSSBranchSubUsersBatchDAOManager : Repository<PSSBranchSubUsersUploadBatchStaging>, IPSSBranchSubUsersBatchDAOManager
    {
        public PSSBranchSubUsersBatchDAOManager(IUoW uow) : base(uow)
        {

        }

        /// <summary>
        /// Gets processing status and filepath of PSSBranchSubUsers upload batch with specified id.
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns>PSSBranchSubUsersBatchDetailsVM</returns>
        public PSSBranchSubUsersBatchDetailsVM GetPSSBranchSubUsersUploadBatchStatusAndFilePath(long batchId)
        {
            try
            {
                return _uow.Session.Query<PSSBranchSubUsersUploadBatchStaging>().Where(x => x.Id == batchId).Select(x => new PSSBranchSubUsersBatchDetailsVM { Status = (PSSBranchSubUserUploadStatus)x.Status, FilePath = x.FilePath }).SingleOrDefault();
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Update processing status for PSSBranchSubUsers upload with specified batch id using the specified processing status.
        /// </summary>
        /// <param name="status"></param>
        /// <param name="batchId"></param>
        /// <param name="errorMessage"></param>
        public void UpdatePSSBranchSubUsersUploadBatchStatus(PSSBranchSubUserUploadStatus status, long batchId, string errorMessage)
        {
            try
            {
                var queryText = $"UPDATE Parkway_CBS_Police_Core_PSSBranchSubUsersUploadBatchStaging SET Status = :status, ErrorMessage = :errorMessage WHERE Id = :batch_Id";
                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("batch_Id", batchId);
                query.SetParameter("status", (int)status);
                query.SetParameter("errorMessage", errorMessage);
                query.ExecuteUpdate();
            }
            catch (Exception) { throw; }
        }
    }
}
