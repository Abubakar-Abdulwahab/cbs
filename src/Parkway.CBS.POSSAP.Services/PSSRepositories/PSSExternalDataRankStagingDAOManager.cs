using NHibernate.Linq;
using Parkway.CBS.ClientRepository;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.POSSAP.Scheduler.Models;
using Parkway.CBS.POSSAP.Services.HelperModel;
using Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Parkway.CBS.POSSAP.Services.PSSRepositories
{
    public class PSSExternalDataRankStagingDAOManager : Repository<PSSExternalDataRankStaging>, IPSSExternalDataRankStagingDAOManager
    {
        public PSSExternalDataRankStagingDAOManager(IUoW uow) : base(uow)
        { }


        /// <summary>
        /// Builds <paramref name="dataTable"/> and query strings for bulk update to <see cref="PSSExternalDataRankStaging"/>
        /// </summary>
        /// <param name="pssExternalDataRankStagingRecords"></param>
        /// <param name="tempTableName"></param>
        /// <param name="createTempTableQuery"></param>
        /// <param name="updateTableQuery"></param>
        /// <param name="dataTable"></param>
        public void BuildPSSExternalDataRankStagingBulkUpdate(IEnumerable<PSSExternalDataRankStagingVM> pssExternalDataRankStagingRecords, out string tempTableName, out string createTempTableQuery, out string updateTableQuery, out DataTable dataTable)
        {
            string tableName = "Parkway_CBS_POSSAP_Scheduler_" + typeof(PSSExternalDataRankStaging).Name;
            tempTableName = $"Temp_{tableName}_{Guid.NewGuid():N}";
            createTempTableQuery = $"CREATE TABLE {tempTableName}([Id] [bigint] NOT NULL, [HasError] [bit] NOT NULL, [ErrorMessage] [nvarchar] (225) NOT NULL);";
            updateTableQuery = $"UPDATE T SET HasError = Temp.HasError, ErrorMessage = Temp.ErrorMessage FROM {tableName} T INNER JOIN {tempTableName} Temp ON T.Id = Temp.Id;";
            dataTable = new DataTable("Parkway_CBS_POSSAP_Scheduler_" + typeof(PSSExternalDataRankStaging).Name);
            dataTable.Columns.Add(new DataColumn(nameof(PSSExternalDataRankStaging.Id), typeof(long)));
            dataTable.Columns.Add(new DataColumn(nameof(PSSExternalDataRankStaging.ErrorMessage), typeof(string)));
            dataTable.Columns.Add(new DataColumn(nameof(PSSExternalDataRankStaging.HasError), typeof(bool)));

            foreach (var rankRecord in pssExternalDataRankStagingRecords)
            {
                DataRow row = dataTable.NewRow();
                row[nameof(PSSExternalDataRankStaging.Id)] = rankRecord.Id;
                row[nameof(PSSExternalDataRankStaging.ErrorMessage)] = rankRecord.ErrorMessage;
                row[nameof(PSSExternalDataRankStaging.HasError)] = rankRecord.HasError;
                dataTable.Rows.Add(row);
            }
        }


        /// <summary>
        /// Gets all the ranks using <paramref name="callLogForExternalSystemId"/>.
        /// </summary>
        /// <param name="callLogForExternalSystemId"></param>
        /// <returns> <see cref="IEnumerable{PSSExternalDataStateStagingVM}"/></returns>
        public IEnumerable<PSSExternalDataRankStagingVM> GetRanks(long callLogForExternalSystemId)
        {
            return _uow.Session.Query<PSSExternalDataRankStaging>()
                 .Where(itm => itm.CallLogForExternalSystem == new CallLogForExternalSystem { Id = callLogForExternalSystemId })
                 .Select(x => new PSSExternalDataRankStagingVM
                 {
                     Id = x.Id,
                     Name = x.Name,
                     Code = x.Code,
                     ExternalDataRankId = x.ExternalDataRankId
                 }).ToFuture();
        }


        /// <summary>
        /// Converts to a datatable <paramref name="rankRecords"/> and saves to the database as a bundle.
        /// </summary>
        /// <param name="rankRecords"></param>
        /// <param name="callLogExternalSystemId"></param>
        /// <exception cref="Exception">Throws an exception if error occurs when saving bundle. </exception>
        public void Save(List<RankReportRecord> rankRecords, long callLogExternalSystemId)
        {
            try
            {
                var dataTable = new DataTable("Parkway_CBS_POSSAP_Scheduler_" + typeof(PSSExternalDataRankStaging).Name);
                dataTable.Columns.Add(new DataColumn(nameof(PSSExternalDataRankStaging.Name), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(PSSExternalDataRankStaging.Code), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(PSSExternalDataRankStaging.ExternalDataRankId), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(PSSExternalDataRankStaging.HasError), typeof(bool)));
                dataTable.Columns.Add(new DataColumn(nameof(PSSExternalDataRankStaging.CallLogForExternalSystem) + "_Id", typeof(long)));
                dataTable.Columns.Add(new DataColumn(nameof(PSSExternalDataRankStaging.CreatedAtUtc), typeof(DateTime)));
                dataTable.Columns.Add(new DataColumn(nameof(PSSExternalDataRankStaging.UpdatedAtUtc), typeof(DateTime)));

                foreach (var rankRecord in rankRecords)
                {
                    DataRow row = dataTable.NewRow();
                    row[nameof(PSSExternalDataRankStaging.Name)] = rankRecord.Name;
                    row[nameof(PSSExternalDataRankStaging.Code)] = rankRecord.Code;
                    row[nameof(PSSExternalDataRankStaging.ExternalDataRankId)] = rankRecord.Id;
                    row[nameof(PSSExternalDataRankStaging.HasError)] = false;
                    row[nameof(PSSExternalDataRankStaging.CallLogForExternalSystem) + "_Id"] = callLogExternalSystemId;
                    row[nameof(PSSExternalDataRankStaging.CreatedAtUtc)] = DateTime.Now.ToLocalTime();
                    row[nameof(PSSExternalDataRankStaging.UpdatedAtUtc)] = DateTime.Now.ToLocalTime();
                    dataTable.Rows.Add(row);
                }


                if (!SaveBundle(dataTable, "Parkway_CBS_POSSAP_Scheduler_" + typeof(PSSExternalDataRankStaging).Name))
                { throw new Exception("Error saving PSS External Data Rank Staging for Call Log External System Id: " + callLogExternalSystemId); }
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// Performs a bulk update using ADO.NET on <see cref="PSSExternalDataRankStaging"/> which requires creating a temporary table <paramref name="tempTableName"/>
        /// </summary>
        /// <param name="stagingBatchItemsDataTable"></param>
        /// <param name="tempTableName"></param>
        /// <param name="createTempQuery"></param>
        /// <param name="updateTableQuery"></param>
        /// <exception cref="Exception">When an error occures</exception>
        public void UpdateErrorMessageAfterValidation(DataTable stagingBatchItemsDataTable, string tempTableName, string createTempQuery, string updateTableQuery)
        {
            try
            {

                IDbConnection connection = _uow.Session.Connection;

                //Creating temp table on database
                var cmd = new SqlCommand
                {
                    CommandTimeout = 10000,
                    Connection = (SqlConnection)connection,
                    CommandText = createTempQuery
                };

                _uow.Session.Transaction.Enlist(cmd);
                SqlConnection serverCon = (SqlConnection)connection;
                cmd.ExecuteNonQuery();

                //Bulk insert into temp table
                SqlBulkCopy copy = new SqlBulkCopy(serverCon, SqlBulkCopyOptions.Default, cmd.Transaction)
                {
                    BulkCopyTimeout = 10000,
                    DestinationTableName = tempTableName
                };

                foreach (DataColumn column in stagingBatchItemsDataTable.Columns) { copy.ColumnMappings.Add(column.ColumnName, column.ColumnName); }
                copy.WriteToServer(stagingBatchItemsDataTable);


                // Updating destination table, and dropping temp table
                cmd.CommandTimeout = 10000;
                cmd.CommandText = updateTableQuery += $" DROP TABLE {tempTableName};";
                cmd.ExecuteNonQuery();

            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// Synchronize rank records in PSSExternalDataRankStaging table with PoliceRanking table
        /// </summary>
        /// <param name="callLogForExternalSystemId"></param>
        public void UpdatePoliceRankingTable(long callLogForExternalSystemId)
        {
            try
            {
                var queryText = "MERGE Parkway_CBS_Police_Core_PoliceRanking AS Target USING Parkway_CBS_POSSAP_Scheduler_PSSExternalDataRankStaging AS Source ON Target.ExternalDataRankId = Source.ExternalDataRankId " +
                                "WHEN MATCHED AND Source.CallLogForExternalSystem_Id = :callLogForExternalSystemId AND Source.HasError = :boolVal THEN UPDATE SET Target.ExternalDataCode = Source.Code, Target.ExternalDataRankId = Source.ExternalDataRankId, Target.UpdatedAtUtc = GETDATE() " +
                                "WHEN NOT MATCHED BY Target AND Source.CallLogForExternalSystem_Id = :callLogForExternalSystemId AND Source.HasError = :boolVal THEN INSERT(RankName, IsActive, CreatedAtUtc, UpdatedAtUtc, ExternalDataRankId, ExternalDataCode)" +
                                "VALUES(Source.Name, 'true', GETDATE(), GETDATE(), Source.ExternalDataRankId, Source.Code);";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("callLogForExternalSystemId", callLogForExternalSystemId);
                query.SetParameter("boolVal", false);
                query.ExecuteUpdate();

            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
