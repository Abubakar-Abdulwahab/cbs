using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.POSSAP.Scheduler.Models;
using Parkway.CBS.POSSAP.Services.HelperModel;
using System;
using System.Collections.Generic;
using System.Data;

namespace Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts
{
    public interface IPSSExternalDataLGAStagingDAOManager : IRepository<PSSExternalDataLGAStaging>
    {
        /// <summary>
        /// Builds <paramref name="dataTable"/> and query strings for bulk update to <see cref="PSSExternalDataLGAStaging"/>
        /// </summary>
        /// <param name="pssExternalDataLGAStagings"></param>
        /// <param name="tempTableName"></param>
        /// <param name="createTempTableQuery"></param>
        /// <param name="updateTableQuery"></param>
        /// <param name="dataTable"></param>
        void BuildPSSExternalDataLGAStagingBulkUpdate(IEnumerable<PSSExternalDataLGAStagingVM> pssExternalDataLGAStagings, out string tempTableName, out string createTempTableQuery, out string updateTableQuery, out DataTable dataTable);

        /// <summary>
        /// Get the list of lgas using <paramref name="callLogForExternalSystemId"/>.
        /// </summary>
        /// <param name="callLogForExternalSystemId"></param>
        /// <returns> <see cref="IEnumerable{PSSExternalDataLGAStagingVM}"/></returns>
        IEnumerable<PSSExternalDataLGAStagingVM> GetListOfLGA(long callLogExternalSystemEntityId);

        /// <summary>
        /// Moves LGA records that do not exist in <see cref="Core.Models.LGA"/> but exists in <see cref="PSSExternalDataLGAStaging.Name"/> 
        /// using <see cref="PSSExternalDataLGAStaging.Name"/> is not equals <see cref="Models.LGAModel.Name"/>
        /// and <paramref name="callLogForExternalSystemId"/>
        /// </summary>
        /// <param name="callLogForExternalSystemId"></param>
        void MoveLGARecordsThatDonotExistToLGAModel(long callLogExternalSystemEntityId, long lastStateCallLogForExternalSystemId);

        /// <summary>
        /// Moves LGA records to <see cref="PSSLGAModelExternalDataLGA"/> when <see cref="PSSExternalDataLGAStaging.Name"/> equals <see cref="Models.LGAModel.Name"/>
        /// using <paramref name="callLogForExternalSystemId"/>
        /// </summary>
        /// <param name="callLogForExternalSystemId"></param>
        void MoveMatchingLGANameToMain(long callLogForExternalSystemId);

        /// <summary>
        /// Converts to a datatable <paramref name="lgaRecords"/> and saves to the database as a bundle.
        /// </summary>
        /// <param name="lgaRecords"></param>
        /// <param name="callLogExternalSystemId"></param>
        /// <exception cref="Exception">Throws an exception if error occurs when saving bundle. </exception>
        void Save(List<LGAReportRecord> lgaRecords, long callLogExternalSystemId);

        /// <summary>
        /// Update lga records in <see cref="Core.Models.LGA"/>  to inactive that do not exists in <see cref="PSSExternalDataLGAStaging"/> 
        /// using <see cref="PSSExternalDataLGAStaging.Name"/> is not equals <see cref="Core.Models.LGA.Name"/>
        /// </summary>
        void SetLGARecordsToInactiveInLGAIfNotExistInStaging(long callLogExternalSystemEntityId);

        /// <summary>
        /// Performs a bulk update using ADO.NET on <see cref="PSSExternalDataLGAStaging"/> which requires creating a temporary table <paramref name="tempTableName"/>
        /// </summary>
        /// <param name="stagingBatchItemsDataTable"></param>
        /// <param name="tempTableName"></param>
        /// <param name="createTempQuery"></param>
        /// <exception cref="Exception">When an error occures</exception>
        void UpdateErrorMessageAfterValidation(DataTable stagingBatchItemsDataTable, string tempTableName, string createTempQuery, string updateTableQuery);
    }
}
