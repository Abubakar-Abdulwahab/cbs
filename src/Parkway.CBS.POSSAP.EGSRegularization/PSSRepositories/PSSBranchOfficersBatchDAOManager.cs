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
    public class PSSBranchOfficersBatchDAOManager : Repository<PSSBranchOfficersUploadBatchStaging>, IPSSBranchOfficersBatchDAOManager
    {
        public PSSBranchOfficersBatchDAOManager(IUoW uow) : base(uow)
        {

        }

        /// <summary>
        /// Gets processing status and filepath of PSSBranchOfficers upload batch with specified id.
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns>PSSBranchOfficersBatchDetailsVM</returns>
        public PSSBranchOfficersBatchDetailsVM GetPSSBranchOfficersUploadBatchStatusAndFilePath(long batchId)
        {
            try
            {
                return _uow.Session.Query<PSSBranchOfficersUploadBatchStaging>().Where(x => x.Id == batchId).Select(x => new PSSBranchOfficersBatchDetailsVM { Status = (PSSBranchOfficersUploadStatus)x.Status, FilePath = x.FilePath }).SingleOrDefault();
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Update processing status for PSSBranchOfficers upload with specified batch id.
        /// </summary>
        /// <param name="status"></param>
        public void UpdatePSSBranchOfficersUploadBatchStatus(long batchId, string errorMessage)
        {
            try
            {
                var queryText = $"UPDATE Parkway_CBS_Police_Core_PSSBranchOfficersUploadBatchStaging SET Status = :status, ErrorMessage = :errorMessage, HasError = :hasError  WHERE Id = :batch_Id";
                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("batch_Id", batchId);
                query.SetParameter("status", (int)PSSBranchOfficersUploadStatus.Fail);
                query.SetParameter("errorMessage", errorMessage);
                query.SetParameter("hasError", true);
                query.ExecuteUpdate();
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Update processing status for PSSBranchOfficers upload with specified batch id using the specified processing status.
        /// </summary>
        /// <param name="status"></param>
        /// <param name="batchId"></param>
        /// <param name="errorMessage"></param>
        public void UpdatePSSBranchOfficersUploadBatchStatus(PSSBranchOfficersUploadStatus status, long batchId, string errorMessage)
        {
            try
            {
                var queryText = $"UPDATE Parkway_CBS_Police_Core_PSSBranchOfficersUploadBatchStaging SET Status = :status, ErrorMessage = :errorMessage WHERE Id = :batch_Id";
                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("batch_Id", batchId);
                query.SetParameter("errorMessage", errorMessage);
                query.SetParameter("status", (int)status);
                query.ExecuteUpdate();
            }
            catch (Exception) { throw; }
        }
    }
}
