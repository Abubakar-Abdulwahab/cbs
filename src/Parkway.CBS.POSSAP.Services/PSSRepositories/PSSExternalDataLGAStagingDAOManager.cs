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
    public class PSSExternalDataLGAStagingDAOManager : Repository<PSSExternalDataLGAStaging>, IPSSExternalDataLGAStagingDAOManager
    {
        public PSSExternalDataLGAStagingDAOManager(IUoW uow) : base(uow)
        { }

        /// <summary>
        /// Build LGA <paramref name="dataTable"/> and query strings for bulk update to <see cref="PSSExternalDataLGAStaging"/>
        /// </summary>
        /// <param name="pssExternalDataLGAStagings"></param>
        /// <param name="tempTableName"></param>
        /// <param name="createTempTableQuery"></param>
        /// <param name="updateTableQuery"></param>
        /// <param name="dataTable"></param>
        public void BuildPSSExternalDataLGAStagingBulkUpdate(IEnumerable<PSSExternalDataLGAStagingVM> pssExternalDataLGAStagings, out string tempTableName, out string createTempTableQuery, out string updateTableQuery, out DataTable dataTable)
        {
            string tableName = "Parkway_CBS_POSSAP_Scheduler_" + typeof(PSSExternalDataLGAStaging).Name;
            tempTableName = $"Temp_{tableName}_{Guid.NewGuid():N}";
            createTempTableQuery = $"CREATE TABLE {tempTableName}([Id] [bigint] NOT NULL, [HasErorr] [bit] NOT NULL, [ErrorMessage] [nvarchar] (225) NOT NULL);";
            updateTableQuery = $"UPDATE T SET HasError = Temp.HasError, ErrorMessage = Temp.ErrorMessage FROM {tableName} T INNER JOIN {tempTableName} Temp ON T.Id = Temp.Id;";
            dataTable = new DataTable("Parkway_CBS_POSSAP_Scheduler_" + typeof(PSSExternalDataLGAStaging).Name);
            dataTable.Columns.Add(new DataColumn(nameof(PSSExternalDataLGAStaging.Id), typeof(long)));
            dataTable.Columns.Add(new DataColumn(nameof(PSSExternalDataLGAStaging.ErrorMessage), typeof(string)));
            dataTable.Columns.Add(new DataColumn(nameof(PSSExternalDataLGAStaging.HasError), typeof(bool)));

            foreach (var lgaRecord in pssExternalDataLGAStagings)
            {
                DataRow row = dataTable.NewRow();
                row[nameof(PSSExternalDataLGAStaging.Id)] = lgaRecord.Id;
                row[nameof(PSSExternalDataLGAStaging.ErrorMessage)] = lgaRecord.ErrorMessage;
                row[nameof(PSSExternalDataLGAStaging.HasError)] = lgaRecord.HasError;
                dataTable.Rows.Add(row);
            }
        }

        /// <summary>
        /// Get the list of lgas using <paramref name="callLogForExternalSystemId"/>.
        /// </summary>
        /// <param name="callLogForExternalSystemId"></param>
        /// <returns> <see cref="IEnumerable{PSSExternalDataLGAStagingVM}"/></returns>
        public IEnumerable<PSSExternalDataLGAStagingVM> GetListOfLGA(long callLogForExternalSystemId)
        {
            return _uow.Session.Query<PSSExternalDataLGAStaging>()
                 .Where(itm => itm.CallLogForExternalSystem == new CallLogForExternalSystem { Id = callLogForExternalSystemId })
                 .Select(ss => new PSSExternalDataLGAStagingVM
                 {
                     Id = ss.Id,
                     Name = ss.Name,
                     Code = ss.Code,
                     StateCode = ss.StateCode,
                 }).ToFuture();
        }

        /// <summary>
        /// Moves LGA record that do not exist in <see cref="Core.Models.LGA"/> but exists in <see cref="PSSExternalDataLGAStaging.Name"/> 
        /// using <see cref="PSSExternalDataLGAStaging.Name"/> is not equals <see cref="Models.LGAModel.Name"/>
        /// and <paramref name="callLogForExternalSystemId"/>
        /// </summary>
        /// <param name="callLogForExternalSystemId"></param>
        /// <param name="lastStateCallLogForExternalSystemId"></param>
        public void MoveLGARecordsThatDonotExistToLGAModel(long callLogExternalSystemEntityId, long lastStateCallLogForExternalSystemId)
        {
            try
            {
                var queryText = "MERGE Parkway_CBS_Core_LGA MLGA USING Parkway_CBS_POSSAP_Scheduler_PSSExternalDataLGAStaging DLGA " +
                                "INNER JOIN Parkway_CBS_POSSAP_Scheduler_PSSStateModelExternalDataState EDS ON EDS.ExternalDataStateCode = DLGA.StateCode AND EDS.CallLogForExternalSystem_Id = :lastStateCallLogForExternalSystemId " +
                                "ON(DLGA.Name = MLGA.Name) " +
                                "WHEN NOT MATCHED BY TARGET AND DLGA.CallLogForExternalSystem_Id = :callLogForExternalSystemId AND DLGA.HasError = :boolVal " +
                                "THEN INSERT(Name, State_Id, CodeName, CreatedAtUtc, UpdatedAtUtc) " +
                                "VALUES(DLGA.Name,EDS.State_Id, UPPER(DLGA.Name), CURRENT_TIMESTAMP, CURRENT_TIMESTAMP) " +
                                "OUTPUT inserted.Id AS LGA_Id, inserted.State_Id AS State_Id, DLGA.StateCode AS ExternalDataLGAStateCode, DLGA.Code AS ExternalDataLGACode, :callLogForExternalSystemId AS CallLogForExternalSystem_Id,  CURRENT_TIMESTAMP AS CreatedAtUtc, CURRENT_TIMESTAMP AS UpdatedAtUtc " +
                                "INTO Parkway_CBS_POSSAP_Scheduler_PSSLGAModelExternalDataLGA(LGA_Id,State_Id, ExternalDataLGAStateCode, ExternalDataLGACode, CallLogForExternalSystem_Id, CreatedAtUtc, UpdatedAtUtc);";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("callLogForExternalSystemId", callLogExternalSystemEntityId);
                query.SetParameter("lastStateCallLogForExternalSystemId", lastStateCallLogForExternalSystemId);
                query.SetParameter("boolVal", false);
                query.ExecuteUpdate();

            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Moves LGA recorDLGA to <see cref="PSSLGAModelExternalDataLGA"/> when <see cref="PSSExternalDataLGAStaging.Name"/> equals <see cref="Models.LGAModel.Name"/>
        /// using <paramref name="callLogForExternalSystemId"/>
        /// </summary>
        /// <param name="callLogForExternalSystemId"></param>
        public void MoveMatchingLGANameToMain(long callLogForExternalSystemId)
        {
            try
            {
                var queryText = $"INSERT INTO Parkway_CBS_POSSAP_Scheduler_PSSLGAModelExternalDataLGA (LGA_Id, State_Id, CallLogForExternalSystem_Id, ExternalDataLGAStateCode, ExternalDataLGACode, CreatedAtUtc, UpdatedAtUtc) " +
                                $"SELECT lga.Id, lga.State_Id, :callLogForExternalSystemId, edlga.StateCode, edlga.Code, edlga.CreatedAtUtc, edlga.UpdatedAtUtc FROM Parkway_CBS_Core_LGA lga " +
                                $"INNER JOIN Parkway_CBS_POSSAP_Scheduler_PSSExternalDataLGAStaging as edlga ON lga.Name = edlga.Name " +
                                $"WHERE edlga.CallLogForExternalSystem_Id = :callLogForExternalSystemId AND edlga.HasError = :boolVal;";

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
        /// Update lga records in <see cref="Core.Models.LGA"/>  to inactive that do not exists in <see cref="PSSExternalDataLGAStaging"/> 
        /// using <see cref="PSSExternalDataLGAStaging.Name"/> is not equals <see cref="Core.Models.LGA.Name"/>
        /// </summary>
        public void SetLGARecordsToInactiveInLGAIfNotExistInStaging(long callLogForExternalSystemId)
        {
            try
            {
                var queryText = "UPDATE lga SET lga.IsActive = :boolVal " +
                                "FROM Parkway_CBS_Core_LGA lga LEFT JOIN Parkway_CBS_POSSAP_Scheduler_PSSExternalDataLGAStaging elga ON lga.Name = elga.Name " +
                                "AND elga.CallLogForExternalSystem_Id = :callLogForExternalSystemId WHERE elga.Name IS NULL;";

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
        /// Converts to a datatable <paramref name="lgaRecorDLGA"/> and saves to the database as a bundle.
        /// </summary>
        /// <param name="lgaRecorDLGA"></param>
        /// <param name="callLogExternalSystemId"></param>
        /// <exception cref="Exception">Throws an exception if error occurs when saving bundle. </exception>
        public void Save(List<LGAReportRecord> lgaRecorDLGA, long callLogExternalSystemId)
        {
            try
            {
                var dataTable = new DataTable("Parkway_CBS_POSSAP_Scheduler_" + typeof(PSSExternalDataLGAStaging).Name);
                dataTable.Columns.Add(new DataColumn(nameof(PSSExternalDataLGAStaging.Name), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(PSSExternalDataLGAStaging.Code), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(PSSExternalDataLGAStaging.StateCode), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(PSSExternalDataLGAStaging.HasError), typeof(bool)));
                dataTable.Columns.Add(new DataColumn(nameof(PSSExternalDataLGAStaging.CallLogForExternalSystem) + "_Id", typeof(long)));
                dataTable.Columns.Add(new DataColumn(nameof(PSSExternalDataLGAStaging.CreatedAtUtc), typeof(DateTime)));
                dataTable.Columns.Add(new DataColumn(nameof(PSSExternalDataLGAStaging.UpdatedAtUtc), typeof(DateTime)));

                foreach (var lgaRecord in lgaRecorDLGA)
                {
                    DataRow row = dataTable.NewRow();
                    row[nameof(PSSExternalDataLGAStaging.Name)] = lgaRecord.Name;
                    row[nameof(PSSExternalDataLGAStaging.Code)] = lgaRecord.Code;
                    row[nameof(PSSExternalDataLGAStaging.StateCode)] = lgaRecord.StateCode;
                    row[nameof(PSSExternalDataLGAStaging.HasError)] = false;
                    row[nameof(PSSExternalDataLGAStaging.CallLogForExternalSystem) + "_Id"] = callLogExternalSystemId;
                    row[nameof(PSSExternalDataLGAStaging.CreatedAtUtc)] = DateTime.Now.ToLocalTime();
                    row[nameof(PSSExternalDataLGAStaging.UpdatedAtUtc)] = DateTime.Now.ToLocalTime();
                    dataTable.Rows.Add(row);
                }


                if (!SaveBundle(dataTable, "Parkway_CBS_POSSAP_Scheduler_" + typeof(PSSExternalDataLGAStaging).Name))
                { throw new Exception("Error saving PSS External Data LGA Staging for Call Log External System Id: " + callLogExternalSystemId); }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Performs a bulk update using ADO.NET on <see cref="PSSExternalDataLGAStaging"/> which requires creating a temporary table <paramref name="tempTableName"/>
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
    }
}
