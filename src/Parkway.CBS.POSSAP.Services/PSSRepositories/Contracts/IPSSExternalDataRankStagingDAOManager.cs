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
    public interface IPSSExternalDataRankStagingDAOManager : IRepository<PSSExternalDataRankStaging>
    {
        /// <summary>
        /// Builds <paramref name="dataTable"/> and query strings for bulk update to <see cref="PSSExternalDataRankStaging"/>
        /// </summary>
        /// <param name="pssExternalDataRankStagingRecords"></param>
        /// <param name="tempTableName"></param>
        /// <param name="createTempTableQuery"></param>
        /// <param name="updateTableQuery"></param>
        /// <param name="dataTable"></param>
        void BuildPSSExternalDataRankStagingBulkUpdate(IEnumerable<PSSExternalDataRankStagingVM> pssExternalDataRankStagingRecords, out string tempTableName, out string createTempTableQuery, out string updateTableQuery, out DataTable dataTable);


        /// <summary>
        /// Converts to a datatable <paramref name="rankRecords"/> and saves to the database as a bundle.
        /// </summary>
        /// <param name="rankRecords"></param>
        /// <param name="callLogExternalSystemId"></param>
        /// <exception cref="Exception">Throws an exception if error occurs when saving bundle. </exception>
        void Save(List<RankReportRecord> rankRecords, long callLogExternalSystemId);


        /// <summary>
        /// Gets all the ranks using <paramref name="callLogForExternalSystemId"/>.
        /// </summary>
        /// <param name="callLogForExternalSystemId"></param>
        /// <returns> <see cref="IEnumerable{PSSExternalDataRankStagingVM}"/></returns>
        IEnumerable<PSSExternalDataRankStagingVM> GetRanks(long callLogForExternalSystemId);


        /// <summary>
        /// Performs a bulk update using ADO.NET on <see cref="PSSExternalDataRankStaging"/> which requires creating a temporary table <paramref name="tempTableName"/>
        /// </summary>
        /// <param name="stagingBatchItemsDataTable"></param>
        /// <param name="tempTableName"></param>
        /// <param name="createTempQuery"></param>
        /// <param name="updateTableQuery"></param>
        /// <exception cref="Exception">When an error occures</exception>
        void UpdateErrorMessageAfterValidation(DataTable stagingBatchItemsDataTable, string tempTableName, string createTempQuery, string updateTableQuery);


        /// <summary>
        /// Synchronize rank records in PSSExternalDataRankStaging table with PoliceRanking table
        /// </summary>
        /// <param name="callLogForExternalSystemId"></param>
        void UpdatePoliceRankingTable(long callLogForExternalSystemId);
    }
}
