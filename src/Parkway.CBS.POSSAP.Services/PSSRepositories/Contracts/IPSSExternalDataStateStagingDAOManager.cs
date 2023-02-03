using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.POSSAP.Scheduler.Models;
using Parkway.CBS.POSSAP.Services.HelperModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts
{
    public interface IPSSExternalDataStateStagingDAOManager : IRepository<PSSExternalDataStateStaging>
    {
        /// <summary>
        /// Builds <paramref name="dataTable"/> and query strings for bulk update to <see cref="PSSExternalDataStateStaging"/>
        /// </summary>
        /// <param name="pssExternalDataStateStagings"></param>
        /// <param name="tempTableName"></param>
        /// <param name="createTempTableQuery"></param>
        /// <param name="updateTableQuery"></param>
        /// <param name="dataTable"></param>
        void BuildPSSExternalDataStateStagingBulkUpdate(IEnumerable<PSSExternalDataStateStagingVM> pssExternalDataStateStagings, out string tempTableName, out string createTempTableQuery, out string updateTableQuery, out DataTable dataTable);

        /// <summary>
        /// Get the list of state using <paramref name="callLogForExternalSystemId"/> 
        /// </summary>
        /// <param name="callLogForExternalSystemId"></param>
        /// <returns> <see cref="IEnumerable{PSSExternalDataStateStagingVM}"/></returns>
        IEnumerable<PSSExternalDataStateStagingVM> GetListOfState(long callLogForExternalSystemId);

        /// <summary>
        /// Moves state records to <see cref="PSSStateModelExternalDataState"/> when <see cref="PSSExternalDataStateStaging.Name"/> equals <see cref="Core.Models.StateModel.Name"/>
        /// using <paramref name="callLogForExternalSystemId"/>
        /// </summary>
        /// <param name="callLogForExternalSystemId"></param>
        void MoveMatchingStateNameToMain(long callLogForExternalSystemId);

        /// <summary>
        /// Moves state records that do not exist in <see cref="Core.Models.StateModel"/> but exists in <see cref="PSSExternalDataStateStaging.Name"/> 
        /// using <see cref="PSSExternalDataStateStaging.Name"/> is not equals <see cref="Core.Models.StateModel.Name"/>
        /// and <paramref name="callLogForExternalSystemId"/>
        /// </summary>
        /// <param name="callLogForExternalSystemId"></param>
        void MoveStateRecordsThatDonotExistToStateModel(long callLogForExternalSystemId);

        /// <summary>
        /// Converts to a datatable <paramref name="stateRecords"/> and saves to the database as a bundle.
        /// </summary>
        /// <param name="stateRecords"></param>
        /// <param name="callLogExternalSystemId"></param>
        /// <exception cref="Exception">Throws an exception if error occurs when saving bundle. </exception>
        void Save(List<StateReportRecord> stateRecords, long callLogExternalSystemId);

        /// <summary>
        /// Update state records in <see cref="Core.Models.StateModel"/>  to inactive that do not exists in <see cref="PSSExternalDataStateStaging"/> 
        /// using <see cref="PSSExternalDataStateStaging.Name"/> is not equals <see cref="Core.Models.StateModel.Name"/>
        /// </summary>
        /// <param name="callLogForExternalSystemId"></param>
        void SetStateRecordsToInactiveInStateModelIfNotExistInStaging(long callLogForExternalSystemId);

        /// <summary>
        /// Iterates and updates the value of <see cref="PSSExternalDataStateStaging.ErrorMessage"/> and <see cref="PSSExternalDataStateStaging.HasErorr"/>
        /// </summary>
        /// <param name="stagingBatchItemsDataTable"></param>
        /// <param name="tempTableName"></param>
        /// <param name="createTempQuery"></param>
        /// <param name="updateTableQuery"></param>
        void UpdateErrorMessageAfterValidation(DataTable stagingBatchItemsDataTable, string tempTableName, string createTempQuery, string updateTableQuery);

    }
}
