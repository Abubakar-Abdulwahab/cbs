using Parkway.CBS.Core.Models;

namespace Parkway.CBS.ClientRepository.Repositories.Contracts
{
    public interface IDirectAssessmentPayeeRecordDAOManager : IRepository<DirectAssessmentPayeeRecord>
    {

        void MigrateIPPISEmployeeRecordsToDirectAssessmentPayeeRecordTable(long batchId, int month, string monthVal, int year);
    }
}
