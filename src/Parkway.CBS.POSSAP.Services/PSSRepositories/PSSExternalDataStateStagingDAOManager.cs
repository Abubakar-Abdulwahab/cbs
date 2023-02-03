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
    public class PSSExternalDataStateStagingDAOManager : Repository<PSSExternalDataStateStaging>, IPSSExternalDataStateStagingDAOManager
    {
        public PSSExternalDataStateStagingDAOManager(IUoW uow) : base(uow)
        { }

        /// <summary>
        /// Builds <paramref name="dataTable"/> and query strings for bulk update to <see cref="PSSExternalDataStateStaging"/>
        /// </summary>
        /// <param name="pssExternalDataStateStagings"></param>
        /// <param name="tempTableName"></param>
        /// <param name="createTempTableQuery"></param>
        /// <param name="updateTableQuery"></param>
        /// <param name="dataTable"></param>
        public void BuildPSSExternalDataStateStagingBulkUpdate(IEnumerable<PSSExternalDataStateStagingVM> pssExternalDataStateStagings, out string tempTableName, out string createTempTableQuery, out string updateTableQuery, out DataTable dataTable)
        {
            string tableName = "Parkway_CBS_POSSAP_Scheduler_" + typeof(PSSExternalDataStateStaging).Name;
            tempTableName = $"Temp_{tableName}_{Guid.NewGuid():N}";
            createTempTableQuery = $"CREATE TABLE {tempTableName}([Id] [bigint] NOT NULL, [HasErorr] [bit] NOT NULL, [ErrorMessage] [nvarchar] (225) NOT NULL);";
            updateTableQuery = $"UPDATE T SET HasErorr = Temp.HasErorr, ErrorMessage = Temp.ErrorMessage FROM {tableName} T INNER JOIN {tempTableName} Temp ON T.Id = Temp.Id;";
            dataTable = new DataTable("Parkway_CBS_POSSAP_Scheduler_" + typeof(PSSExternalDataStateStaging).Name);
            dataTable.Columns.Add(new DataColumn(nameof(PSSExternalDataStateStaging.Id), typeof(long)));
            dataTable.Columns.Add(new DataColumn(nameof(PSSExternalDataStateStaging.ErrorMessage), typeof(string)));
            dataTable.Columns.Add(new DataColumn(nameof(PSSExternalDataStateStaging.HasErorr), typeof(bool)));

            foreach (var stateRecord in pssExternalDataStateStagings)
            {
                DataRow row = dataTable.NewRow();
                row[nameof(PSSExternalDataStateStaging.Id)] = stateRecord.Id;
                row[nameof(PSSExternalDataStateStaging.ErrorMessage)] = stateRecord.ErrorMessage;
                row[nameof(PSSExternalDataStateStaging.HasErorr)] = stateRecord.HasError;
                dataTable.Rows.Add(row);
            }
        }

        /// <summary>
        /// Get the list of state using <paramref name="callLogForExternalSystemId"/>.
        /// </summary>
        /// <param name="callLogForExternalSystemId"></param>
        /// <returns> <see cref="IEnumerable{PSSExternalDataStateStagingVM}"/></returns>
        public IEnumerable<PSSExternalDataStateStagingVM> GetListOfState(long callLogForExternalSystemId)
        {
            return _uow.Session.Query<PSSExternalDataStateStaging>()
                 .Where(itm => itm.CallLogForExternalSystem == new CallLogForExternalSystem { Id = callLogForExternalSystemId })
                 .Select(ss => new PSSExternalDataStateStagingVM
                 {
                     Id = ss.Id,
                     Name = ss.Name,
                     Code = ss.Code,
                 }).ToFuture();
        }

        /// <summary>
        /// Moves state records to <see cref="PSSStateModelExternalDataState"/> when <see cref="PSSExternalDataStateStaging.Name"/> equals <see cref="Core.Models.StateModel.Name"/>
        /// using <paramref name="callLogForExternalSystemId"/>
        /// </summary>
        /// <param name="callLogForExternalSystemId"></param>
        public void MoveMatchingStateNameToMain(long callLogForExternalSystemId)
        {
            try
            {
                var queryText = $"INSERT INTO Parkway_CBS_POSSAP_Scheduler_PSSStateModelExternalDataState (State_Id, CallLogForExternalSystem_Id, ExternalDataStateCode, CreatedAtUtc, UpdatedAtUtc) " +
                                $"SELECT sm.Id, :callLogForExternalSystemId, edss.Code, edss.CreatedAtUtc, edss.UpdatedAtUtc FROM Parkway_CBS_Core_StateModel sm " +
                                $"INNER JOIN Parkway_CBS_POSSAP_Scheduler_PSSExternalDataStateStaging as edss ON sm.Name = edss.Name " +
                                $"WHERE edss.CallLogForExternalSystem_Id = :callLogForExternalSystemId AND edss.HasErorr = :boolVal;";

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

        /// <summary>
        /// Moves state records that do not exist in <see cref="Core.Models.StateModel"/> but exists in <see cref="PSSExternalDataStateStaging.Name"/> 
        /// using <see cref="PSSExternalDataStateStaging.Name"/> is not equals <see cref="Core.Models.StateModel.Name"/>
        /// and <paramref name="callLogForExternalSystemId"/>
        /// And also inserts the records to <see cref="PSSStateModelExternalDataState"/>
        /// </summary>
        /// <param name="callLogForExternalSystemId"></param>
        public void MoveStateRecordsThatDonotExistToStateModel(long callLogForExternalSystemId)
        {
            try
            {
                var queryText = "MERGE Parkway_CBS_Core_StateModel MS USING Parkway_CBS_POSSAP_Scheduler_PSSExternalDataStateStaging DS ON(DS.Name = MS.Name) " +
                                "WHEN NOT MATCHED BY TARGET AND DS.CallLogForExternalSystem_Id = :callLogForExternalSystemId AND DS.HasErorr = :boolVal " +
                                "THEN INSERT(Name, ShortName, CreatedAtUtc, UpdatedAtUtc) " +
                                "VALUES(DS.Name, UPPER(LEFT(DS.Name, 3)), CURRENT_TIMESTAMP, CURRENT_TIMESTAMP) " +
                                "OUTPUT inserted.Id AS State_Id, DS.Code AS ExternalDataStateCode, :callLogForExternalSystemId AS CallLogForExternalSystem_Id,  CURRENT_TIMESTAMP AS CreatedAtUtc, CURRENT_TIMESTAMP AS UpdatedAtUtc " +
                                "INTO Parkway_CBS_POSSAP_Scheduler_PSSStateModelExternalDataState(State_Id, ExternalDataStateCode, CallLogForExternalSystem_Id, CreatedAtUtc, UpdatedAtUtc);";

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

        /// <summary>
        /// Converts to a datatable <paramref name="stateRecords"/> and saves to the database as a bundle.
        /// </summary>
        /// <param name="stateRecords"></param>
        /// <param name="callLogExternalSystemId"></param>
        /// <exception cref="Exception">Throws an exception if error occurs when saving bundle. </exception>
        public void Save(List<StateReportRecord> stateRecords, long callLogExternalSystemId)
        {
            try
            {
                var dataTable = new DataTable("Parkway_CBS_POSSAP_Scheduler_" + typeof(PSSExternalDataStateStaging).Name);
                dataTable.Columns.Add(new DataColumn(nameof(PSSExternalDataStateStaging.Name), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(PSSExternalDataStateStaging.Code), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(PSSExternalDataStateStaging.HasErorr), typeof(bool)));
                dataTable.Columns.Add(new DataColumn(nameof(PSSExternalDataStateStaging.CallLogForExternalSystem) + "_Id", typeof(long)));
                dataTable.Columns.Add(new DataColumn(nameof(PSSExternalDataStateStaging.CreatedAtUtc), typeof(DateTime)));
                dataTable.Columns.Add(new DataColumn(nameof(PSSExternalDataStateStaging.UpdatedAtUtc), typeof(DateTime)));

                foreach (var stateRecord in stateRecords)
                {
                    DataRow row = dataTable.NewRow();
                    row[nameof(PSSExternalDataStateStaging.Name)] = stateRecord.Name;
                    row[nameof(PSSExternalDataStateStaging.Code)] = stateRecord.Code;
                    row[nameof(PSSExternalDataStateStaging.HasErorr)] = false;
                    row[nameof(PSSExternalDataStateStaging.CallLogForExternalSystem) + "_Id"] = callLogExternalSystemId;
                    row[nameof(PSSExternalDataStateStaging.CreatedAtUtc)] = DateTime.Now.ToLocalTime();
                    row[nameof(PSSExternalDataStateStaging.UpdatedAtUtc)] = DateTime.Now.ToLocalTime();
                    dataTable.Rows.Add(row);
                }


                if (!SaveBundle(dataTable, "Parkway_CBS_POSSAP_Scheduler_" + typeof(PSSExternalDataStateStaging).Name))
                { throw new Exception("Error saving PSS External Data State Staging for Call Log External System Id: " + callLogExternalSystemId); }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Update state records in <see cref="Core.Models.StateModel"/>  to inactive that do not exists in <see cref="PSSExternalDataStateStaging"/> 
        /// using <see cref="PSSExternalDataStateStaging.Name"/> is not equals <see cref="Core.Models.StateModel.Name"/>
        /// </summary>
        public void SetStateRecordsToInactiveInStateModelIfNotExistInStaging(long callLogForExternalSystemId)
        {
            try
            {
                var queryText = "UPDATE sm SET sm.IsActive = :boolVal " +
                                "FROM Parkway_CBS_Core_StateModel sm LEFT JOIN Parkway_CBS_POSSAP_Scheduler_PSSExternalDataStateStaging edss ON sm.Name = edss.Name " +
                                "AND edss.CallLogForExternalSystem_Id = :callLogForExternalSystemId WHERE edss.Name IS NULL;";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("boolVal", false);
                query.SetParameter("callLogForExternalSystemId", callLogForExternalSystemId);
                query.ExecuteUpdate();

            }
            catch (Exception)
            {
                throw;
            }
        }
       
        /// <summary>
        /// Performs a bulk update using ADO.NET on <see cref="PSSExternalDataStateStaging"/> which requires creating a temporary table <paramref name="tempTableName"/>
        /// </summary>
        /// <param name="stagingBatchItemsDataTable"></param>
        /// <param name="tempTableName"></param>
        /// <param name="createTempQuery"></param>
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
    }
}
