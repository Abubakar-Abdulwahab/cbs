using NHibernate.Linq;
using Parkway.CBS.ClientRepository;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.POSSAP.EGSRegularization.HelperModels;
using Parkway.CBS.POSSAP.EGSRegularization.PSSRepositories.Contracts;
using System;
using System.Linq;

namespace Parkway.CBS.POSSAP.EGSRegularization.PSSRepositories
{
    public class GenerateRequestWithoutOfficersUploadBatchDAOManager : Repository<GenerateRequestWithoutOfficersUploadBatchStaging>, IGenerateRequestWithoutOfficersUploadBatchDAOManager
    {
        public GenerateRequestWithoutOfficersUploadBatchDAOManager(IUoW uow) : base(uow)
        {

        }

        /// <summary>
        /// Gets processing status and filepath of GenerateRequestWithoutOfficers upload batch with specified id.
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns>PSSBranchSubUsersBatchDetailsVM</returns>
        public GenerateRequestWithoutOfficersUploadBatchDetailsVM GetGenerateRequestWithoutOfficersUploadBatchStatusAndFilePath(long batchId)
        {
            try
            {
                return _uow.Session.Query<GenerateRequestWithoutOfficersUploadBatchStaging>().Where(x => x.Id == batchId).Select(x => new GenerateRequestWithoutOfficersUploadBatchDetailsVM { Status = (GenerateRequestWithoutOfficersUploadStatus)x.Status, FilePath = x.FilePath }).SingleOrDefault();
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Update processing status for GenerateRequestWithoutOfficers upload with specified batch id using the specified processing status.
        /// </summary>
        /// <param name="status"></param>
        /// <param name="batchId"></param>
        /// <param name="errorMessage"></param>
        public void UpdateGenerateRequestWithoutOfficersUploadBatchStatus(GenerateRequestWithoutOfficersUploadStatus status, long batchId, string errorMessage)
        {
            try
            {
                var queryText = $"UPDATE Parkway_CBS_Police_Core_GenerateRequestWithoutOfficersUploadBatchStaging SET Status = :status, ErrorMessage = :errorMessage WHERE Id = :batch_Id";
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
