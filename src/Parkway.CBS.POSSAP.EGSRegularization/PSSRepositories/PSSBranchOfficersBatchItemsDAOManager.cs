using NHibernate.Linq;
using Parkway.CBS.ClientRepository;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Police.Core.ExternalSourceData.HRSystem.ViewModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.POSSAP.EGSRegularization.HelperModel;
using Parkway.CBS.POSSAP.EGSRegularization.HelperModels;
using Parkway.CBS.POSSAP.EGSRegularization.PSSRepositories.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Parkway.CBS.POSSAP.EGSRegularization.PSSRepositories
{
    public class PSSBranchOfficersBatchItemsDAOManager : Repository<PSSBranchOfficersUploadBatchItemsStaging>, IPSSBranchOfficersBatchItemsDAOManager
    {
        public PSSBranchOfficersBatchItemsDAOManager(IUoW uow) : base(uow)
        {

        }

        /// <summary>
        /// Save PSSBranchOfficers line items as a bundle
        /// </summary>
        /// <param name="lineItems"></param>
        /// <param name="batchId"></param>
        public void SavePSSBranchOfficersLineItemsRecords(List<PSSBranchOfficersItemVM> lineItems, long batchId)
        {
            //Logger.Information("Saving PSSBranchSubUsers records for batch id " + batchId);
            //save PSSBranchSubUsers line items to table
            try
            {
                var dataTable = new DataTable("Parkway_CBS_Police_Core_" + typeof(PSSBranchOfficersUploadBatchItemsStaging).Name);
                dataTable.Columns.Add(new DataColumn(nameof(PSSBranchOfficersUploadBatchItemsStaging.BranchCode), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(PSSBranchOfficersUploadBatchItemsStaging.BranchCodeValue), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(PSSBranchOfficersUploadBatchItemsStaging.APNumber), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(PSSBranchOfficersUploadBatchItemsStaging.OfficerName), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(PSSBranchOfficersUploadBatchItemsStaging.OfficerCommand) + "_Id", typeof(int)));
                dataTable.Columns.Add(new DataColumn(nameof(PSSBranchOfficersUploadBatchItemsStaging.OfficerCommandValue), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(PSSBranchOfficersUploadBatchItemsStaging.OfficerCommandCode), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(PSSBranchOfficersUploadBatchItemsStaging.RankCode), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(PSSBranchOfficersUploadBatchItemsStaging.RankName), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(PSSBranchOfficersUploadBatchItemsStaging.Rank) + "_Id", typeof(int)));
                dataTable.Columns.Add(new DataColumn(nameof(PSSBranchOfficersUploadBatchItemsStaging.BankCode), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(PSSBranchOfficersUploadBatchItemsStaging.BankName), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(PSSBranchOfficersUploadBatchItemsStaging.PhoneNumber), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(PSSBranchOfficersUploadBatchItemsStaging.IPPISNumber), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(PSSBranchOfficersUploadBatchItemsStaging.Gender), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(PSSBranchOfficersUploadBatchItemsStaging.AccountNumber), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(PSSBranchOfficersUploadBatchItemsStaging.HasError), typeof(bool)));
                dataTable.Columns.Add(new DataColumn(nameof(PSSBranchOfficersUploadBatchItemsStaging.ErrorMessage), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(PSSBranchOfficersUploadBatchItemsStaging.PSSBranchOfficersUploadBatchStaging) + "_Id", typeof(long)));
                dataTable.Columns.Add(new DataColumn(nameof(PSSBranchOfficersUploadBatchItemsStaging.CreatedAtUtc), typeof(DateTime)));
                dataTable.Columns.Add(new DataColumn(nameof(PSSBranchOfficersUploadBatchItemsStaging.UpdatedAtUtc), typeof(DateTime)));

                foreach (var lineItem in lineItems)
                {
                    DataRow row = dataTable.NewRow();
                    row[nameof(PSSBranchOfficersUploadBatchItemsStaging.BranchCode)] = lineItem.BranchCode;
                    row[nameof(PSSBranchOfficersUploadBatchItemsStaging.BranchCodeValue)] = lineItem.BranchCodeValue;
                    row[nameof(PSSBranchOfficersUploadBatchItemsStaging.APNumber)] = lineItem.APNumber;
                    row[nameof(PSSBranchOfficersUploadBatchItemsStaging.OfficerName)] = lineItem.OfficerName;
                    row[nameof(PSSBranchOfficersUploadBatchItemsStaging.OfficerCommand) + "_Id"] = DBNull.Value;
                    row[nameof(PSSBranchOfficersUploadBatchItemsStaging.OfficerCommandValue)] = lineItem.OfficerCommandValue;
                    row[nameof(PSSBranchOfficersUploadBatchItemsStaging.OfficerCommandCode)] = lineItem.OfficerCommandCode;
                    row[nameof(PSSBranchOfficersUploadBatchItemsStaging.RankCode)] = lineItem.RankCode;
                    row[nameof(PSSBranchOfficersUploadBatchItemsStaging.RankName)] = lineItem.RankName;
                    row[nameof(PSSBranchOfficersUploadBatchItemsStaging.Rank) + "_Id"] = DBNull.Value;
                    row[nameof(PSSBranchOfficersUploadBatchItemsStaging.BankCode)] = lineItem.BankCode;
                    row[nameof(PSSBranchOfficersUploadBatchItemsStaging.BankName)] = lineItem.BankName;
                    row[nameof(PSSBranchOfficersUploadBatchItemsStaging.PhoneNumber)] = lineItem.PhoneNumber;
                    row[nameof(PSSBranchOfficersUploadBatchItemsStaging.IPPISNumber)] = lineItem.IPPISNumber;
                    row[nameof(PSSBranchOfficersUploadBatchItemsStaging.Gender)] = lineItem.Gender;
                    row[nameof(PSSBranchOfficersUploadBatchItemsStaging.AccountNumber)] = lineItem.AccountNumber;
                    row[nameof(PSSBranchOfficersUploadBatchItemsStaging.HasError)] = lineItem.HasError;
                    row[nameof(PSSBranchOfficersUploadBatchItemsStaging.ErrorMessage)] = lineItem.ErrorMessage;
                    row[nameof(PSSBranchOfficersUploadBatchItemsStaging.PSSBranchOfficersUploadBatchStaging) + "_Id"] = batchId;
                    row[nameof(PSSBranchOfficersUploadBatchItemsStaging.CreatedAtUtc)] = DateTime.Now.ToLocalTime();
                    row[nameof(PSSBranchOfficersUploadBatchItemsStaging.UpdatedAtUtc)] = DateTime.Now.ToLocalTime();
                    dataTable.Rows.Add(row);
                }

                if (!SaveBundle(dataTable, "Parkway_CBS_Police_Core_" + typeof(PSSBranchOfficersUploadBatchItemsStaging).Name))
                { throw new Exception("Error saving PSSBranchOfficers excel file details for batch Id " + batchId); }
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// Get records from PSSBranchOfficers in batch with specified id and having no validation errors
        /// </summary>
        /// <param name="batchId"></param>
        public IEnumerable<PSSBranchOfficersItemVM> GetValidBranchOfficersByBatchId(long batchId)
        {
            return _uow.Session.Query<PSSBranchOfficersUploadBatchItemsStaging>().Where(x => x.PSSBranchOfficersUploadBatchStaging == new PSSBranchOfficersUploadBatchStaging { Id = batchId } && x.HasError == false).Select(b => new PSSBranchOfficersItemVM
            {
                Id = b.Id,
                HasError = b.HasError,
                APNumber = b.APNumber,
                BranchCode = b.BranchCode,
                BranchCodeValue = b.BranchCodeValue,
                ErrorMessage = b.ErrorMessage,
            });
        }


        /// <summary>
        /// Perform update on PSSBranchSubUsersItem in batch with APNumber specified PersonnelReportRecord
        /// </summary>
        /// <param name="officerDetails"></param>
        /// <param name="itemId"></param>
        public void UpdatePSSBranchOfficersDetails(PersonnelReportRecord officerDetails, long itemId)
        {
            try
            {
                var queryText = $"UPDATE Parkway_CBS_Police_Core_PSSBranchOfficersUploadBatchItemsStaging SET IPPISNumber = :ippisNumber, OfficerCommandCode = :officerCommandCode, " +
                                $"OfficerCommand_Id = :officerCommandId, Rank_Id = :rankId, Gender = :gender, OfficerName = :name, BankCode = :bankCode, AccountNumber = :accountNumber, " +
                                $"PhoneNumber = :phoneNumber, BankName = :bankName, RankName = :rankName, RankCode = :rankCode, OfficerCommandValue = :commandName, " +
                                $"UpdatedAtUtc = :updatedAt WHERE APNumber = :serviceNumber AND Id = :batchItemId; ";
                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("serviceNumber", officerDetails.ServiceNumber);
                query.SetParameter("ippisNumber", officerDetails.IPPSNumber);
                query.SetParameter("gender", officerDetails.Gender);
                query.SetParameter("accountNumber", officerDetails.AccountNumber);
                query.SetParameter("phoneNumber", officerDetails.PhoneNumber);
                query.SetParameter("bankCode", officerDetails.BankCode);
                query.SetParameter("bankName", officerDetails.BankName);
                query.SetParameter("rankName", officerDetails.RankName);
                query.SetParameter("rankCode", officerDetails.RankCode);
                query.SetParameter("phoneNumber", officerDetails.PhoneNumber);
                query.SetParameter("commandName", officerDetails.CommandName);
                query.SetParameter("officerCommandCode", officerDetails.CommandCode);
                query.SetParameter("officerCommandId", int.Parse(officerDetails.CommandLevelCode));
                query.SetParameter("rankId", long.Parse(officerDetails.RankId));
                query.SetParameter("name", string.Format("{0} {1}", officerDetails.FirstName, officerDetails.Surname).ToUpper());
                query.SetParameter("updatedAt", DateTime.Now.ToLocalTime());
                query.SetParameter("batchItemId", itemId);
                query.ExecuteUpdate();
            }
            catch (Exception) { throw; }
        }


        /// <summary>
        /// Update error status for PSSBranchOfficers with specified batch id using the specified error message.
        /// </summary>
        /// <param name="batchOfficerId"></param>
        /// <param name="errorMessage"></param>
        public void UpdatePSSBranchOfficersErrorStatus(string errorMessage, long batchOfficerId)
        {
            try
            {
                var queryText = $"UPDATE Parkway_CBS_Police_Core_PSSBranchOfficersUploadBatchItemsStaging SET HasError = :hasError, ErrorMessage = :errorMessage WHERE Id = :batchOfficerId";

                var query = _uow.Session.CreateSQLQuery(queryText);

                query.SetParameter("hasError", true);
                query.SetParameter("errorMessage", errorMessage);
                query.SetParameter("batchOfficerId", batchOfficerId);
                query.ExecuteUpdate();
            }
            catch (Exception) { throw; }
        }


        /// <summary>
        /// Perform validation on PSSBranchOfficerItems APNumber column that duplicates are not uploaded, updating hasError and errorMessage for items in batch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="errorMessage"></param>
        public void ValidateAPNumberIsNotDuplicateAndUpdatePSSBranchOfficersItemErrorMessage(long batchId, string errorMessage)
        {
            try
            {
                var queryText = $"UPDATE S1 SET S1.{nameof(PSSBranchOfficersUploadBatchItemsStaging.HasError)} = 1, " +
                                $"S1.{nameof(PSSBranchOfficersUploadBatchItemsStaging.ErrorMessage)} = CONCAT(S1.{nameof(PSSBranchOfficersUploadBatchItemsStaging.ErrorMessage)}, :errorMessage) " +
                                $"FROM Parkway_CBS_Police_Core_{nameof(PSSBranchOfficersUploadBatchItemsStaging)} AS S1 " +
                                $"WHERE (SELECT COUNT(S2.{nameof(PSSBranchOfficersUploadBatchItemsStaging.APNumber)}) " +
                                $"FROM Parkway_CBS_Police_Core_{nameof(PSSBranchOfficersUploadBatchItemsStaging)} AS S2 " +
                                $"GROUP BY S2.{nameof(PSSBranchOfficersUploadBatchItemsStaging.PSSBranchOfficersUploadBatchStaging)}_Id, S2.{nameof(PSSBranchOfficersUploadBatchItemsStaging.APNumber)} " +
                                $"HAVING S1.{nameof(PSSBranchOfficersUploadBatchItemsStaging.APNumber)} = S2.{nameof(PSSBranchOfficersUploadBatchItemsStaging.APNumber)} " +
                                $"AND S2.{nameof(PSSBranchOfficersUploadBatchItemsStaging.PSSBranchOfficersUploadBatchStaging)}_Id = :batchId) > 1 " +
                                $"AND S1.{nameof(PSSBranchOfficersUploadBatchItemsStaging.PSSBranchOfficersUploadBatchStaging)}_Id = :batchId " +
                                $"AND S1.{nameof(PSSBranchOfficersUploadBatchItemsStaging.HasError)} = :hasError; ";
                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("batchId", batchId);
                query.SetParameter("hasError", false);
                query.SetParameter("errorMessage", errorMessage);
                query.ExecuteUpdate();
            }
            catch (Exception) { throw; }
        }


        /// <summary>
        /// Perform bulk validation on PSSBranchOfficerItems APNumber column that an officer with same APNumber is not on Active deployment, updating hasError and errorMessage for items in batch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="errorMessage"></param>
        public void ValidateAPNumberIsNotOnActiveDeploymentndUpdatePSSBranchOfficersItemErrorMessage(long batchId, string errorMessage)
        {
            var queryText = $"UPDATE ITEMS SET ITEMS.{nameof(PSSBranchOfficersUploadBatchItemsStaging.HasError)} = 1, " +
                            $"ITEMS.{nameof(PSSBranchOfficersUploadBatchItemsStaging.ErrorMessage)} = CONCAT(ITEMS.{nameof(PSSBranchOfficersUploadBatchItemsStaging.ErrorMessage)}, :errorMessage) " +
                            $"FROM Parkway_CBS_Police_Core_{nameof(PSSBranchOfficersUploadBatchItemsStaging)} AS ITEMS " +
                            $"INNER JOIN Parkway_CBS_Police_Core_{nameof(PolicerOfficerLog)} AS OL ON ITEMS.{nameof(PSSBranchOfficersUploadBatchItemsStaging.APNumber)} = OL.{nameof(PolicerOfficerLog.IdentificationNumber)} " +
                            $"INNER JOIN Parkway_CBS_Police_Core_{nameof(PoliceOfficerDeploymentLog)} AS DL ON DL.{nameof(PoliceOfficerDeploymentLog.PoliceOfficerLog)}_Id = OL.{nameof(PolicerOfficerLog.Id)} " +
                            $"WHERE DL.{nameof(PoliceOfficerDeploymentLog.IsActive)} = :isActive " +
                            $"AND ITEMS.{nameof(PSSBranchOfficersUploadBatchItemsStaging.PSSBranchOfficersUploadBatchStaging)}_Id = :batchId " +
                            $"AND ITEMS.{nameof(PSSBranchOfficersUploadBatchItemsStaging.HasError)} = :hasError; ";
            var query = _uow.Session.CreateSQLQuery(queryText);
            query.SetParameter("isActive", true);
            query.SetParameter("batchId", batchId);
            query.SetParameter("hasError", false);
            query.SetParameter("errorMessage", errorMessage);
            query.ExecuteUpdate();
        }


        /// <summary>
        /// Perform bulk validation on PSSBranchOfficerItems BRANCH CODE column to make sure it is same as current batch branch, updating hasError and errorMessage for items in batch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="errorMessage"></param>
        public void ValidateBranchCodeIsSameAsCurrentBranchAndUpdatePSSBranchOfficersItemErrorMessage(long batchId, string errorMessage)
        {
            var queryText = $"UPDATE ITEMS SET ITEMS.{nameof(PSSBranchOfficersUploadBatchItemsStaging.HasError)} = 1, " +
                            $"ITEMS.{nameof(PSSBranchOfficersUploadBatchItemsStaging.ErrorMessage)} = CONCAT(ITEMS.{nameof(PSSBranchOfficersUploadBatchItemsStaging.ErrorMessage)}, :errorMessage) " +
                            $"FROM Parkway_CBS_Police_Core_{nameof(PSSBranchOfficersUploadBatchItemsStaging)} AS ITEMS " +
                            $"INNER JOIN Parkway_CBS_Police_Core_{nameof(PSSBranchOfficersUploadBatchStaging)} AS BATCH " +
                            $"ON ITEMS.{nameof(PSSBranchOfficersUploadBatchStaging)}_Id = BATCH.{nameof(PSSBranchOfficersUploadBatchStaging.Id)} " +
                            $"INNER JOIN Parkway_CBS_Core_{nameof(TaxEntityProfileLocation)} AS LOC " +
                            $"ON BATCH.{nameof(TaxEntityProfileLocation)}_Id = LOC.{nameof(TaxEntityProfileLocation.Id)} " +
                            $"WHERE LOC.{nameof(TaxEntityProfileLocation.Code)} != ITEMS.{nameof(PSSBranchOfficersUploadBatchItemsStaging.BranchCode)} " +
                            $"AND ITEMS.{nameof(PSSBranchOfficersUploadBatchItemsStaging.PSSBranchOfficersUploadBatchStaging)}_Id = :batchId " +
                            $"AND ITEMS.{nameof(PSSBranchOfficersUploadBatchItemsStaging.HasError)} = :hasError; ";
            var query = _uow.Session.CreateSQLQuery(queryText);
            query.SetParameter("batchId", batchId);
            query.SetParameter("hasError", false);
            query.SetParameter("errorMessage", errorMessage);
            query.ExecuteUpdate();
        }


        /// <summary>
        /// Build PSSBranchOfficersUploadBatchItemsStaging Temp Table for Bulk Update
        /// </summary>
        /// <param name="PSSBranchOfficersUploadBatchItems"></param>
        /// <param name="batchId"></param>
        /// <param name="tempTableName"></param>
        /// <param name="createTempTableQuery"></param>
        /// <param name="updateTableQuery"></param>
        /// <param name="dataTable"></param>
        public void BuildPSSBranchOfficersUploadBatchItemsStagingBulkUpdate(IEnumerable<APNumberValidationVM> PSSBranchOfficersUploadBatchItems, long batchId, out string tempTableName, out string createTempTableQuery, out string updateTableQuery, out DataTable dataTable)
        {
            string tableName = "Parkway_CBS_Police_Core_" + typeof(PSSBranchOfficersUploadBatchItemsStaging).Name;
            tempTableName = $"Temp_{tableName}_{batchId}_{Guid.NewGuid():N}";
            createTempTableQuery = $"CREATE TABLE {tempTableName}([Id] [bigint] NOT NULL, [IPPISNumber] [nvarchar] (225) NULL, [OfficerCommandCode] [nvarchar] (225) NULL, [OfficerCommand_Id] [int] NULL, [Rank_Id] [int] NULL, [Gender] [nvarchar] (225) NULL, [OfficerName] [nvarchar] (225) NULL, [BankCode] [nvarchar] (225) NULL, [AccountNumber] [nvarchar] (225) NULL, [PhoneNumber] [nvarchar] (225) NULL, [BankName] [nvarchar] (225) NULL, [RankName] [nvarchar] (225) NULL, [RankCode] [nvarchar] (225) NULL, [OfficerCommandValue] [nvarchar] (225) NULL, [HasError] [bit] NOT NULL, [ErrorMessage] [nvarchar] (225) NULL);";

            updateTableQuery = $"UPDATE T SET T.IPPISNumber = Temp.IPPISNumber, T.OfficerCommandCode = Temp.OfficerCommandCode, T.OfficerCommand_Id = Temp.OfficerCommand_Id, T.Rank_Id = Temp.Rank_Id, T.Gender = Temp.Gender, T.OfficerName = Temp.OfficerName, T.BankCode = Temp.BankCode, T.AccountNumber = Temp.AccountNumber, T.PhoneNumber = Temp.PhoneNumber, T.BankName = Temp.BankName, T.RankName = Temp.RankName, T.RankCode = Temp.RankCode, T.OfficerCommandValue = Temp.OfficerCommandValue, T.HasError = Temp.HasError, T.ErrorMessage = Temp.ErrorMessage FROM {tableName} AS T INNER JOIN {tempTableName} Temp ON T.Id = Temp.Id;";

            dataTable = new DataTable("Parkway_CBS_Police_Core_" + typeof(PSSBranchOfficersUploadBatchItemsStaging).Name);
            dataTable.Columns.Add(new DataColumn(nameof(PSSBranchOfficersUploadBatchItemsStaging.Id), typeof(long)));
            dataTable.Columns.Add(new DataColumn(nameof(PSSBranchOfficersUploadBatchItemsStaging.IPPISNumber), typeof(string)));
            dataTable.Columns.Add(new DataColumn(nameof(PSSBranchOfficersUploadBatchItemsStaging.OfficerCommandCode), typeof(string)));
            dataTable.Columns.Add(new DataColumn(nameof(PSSBranchOfficersUploadBatchItemsStaging.OfficerCommand) + "_Id", typeof(int)));
            dataTable.Columns.Add(new DataColumn(nameof(PSSBranchOfficersUploadBatchItemsStaging.Rank) + "_Id", typeof(int)));
            dataTable.Columns.Add(new DataColumn(nameof(PSSBranchOfficersUploadBatchItemsStaging.Gender), typeof(string)));
            dataTable.Columns.Add(new DataColumn(nameof(PSSBranchOfficersUploadBatchItemsStaging.OfficerName), typeof(string)));
            dataTable.Columns.Add(new DataColumn(nameof(PSSBranchOfficersUploadBatchItemsStaging.BankCode), typeof(string)));
            dataTable.Columns.Add(new DataColumn(nameof(PSSBranchOfficersUploadBatchItemsStaging.AccountNumber), typeof(string)));
            dataTable.Columns.Add(new DataColumn(nameof(PSSBranchOfficersUploadBatchItemsStaging.PhoneNumber), typeof(string)));
            dataTable.Columns.Add(new DataColumn(nameof(PSSBranchOfficersUploadBatchItemsStaging.BankName), typeof(string)));
            dataTable.Columns.Add(new DataColumn(nameof(PSSBranchOfficersUploadBatchItemsStaging.RankName), typeof(string)));
            dataTable.Columns.Add(new DataColumn(nameof(PSSBranchOfficersUploadBatchItemsStaging.RankCode), typeof(string)));
            dataTable.Columns.Add(new DataColumn(nameof(PSSBranchOfficersUploadBatchItemsStaging.OfficerCommandValue), typeof(string)));
            dataTable.Columns.Add(new DataColumn(nameof(PSSBranchOfficersUploadBatchItemsStaging.ErrorMessage), typeof(string)));
            dataTable.Columns.Add(new DataColumn(nameof(PSSBranchOfficersUploadBatchItemsStaging.HasError), typeof(bool)));

            foreach (var item in PSSBranchOfficersUploadBatchItems)
            {
                DataRow row = dataTable.NewRow();
                if (item.HasError)
                {
                    row[nameof(PSSBranchOfficersUploadBatchItemsStaging.IPPISNumber)] = DBNull.Value;
                    row[nameof(PSSBranchOfficersUploadBatchItemsStaging.OfficerCommandCode)] = DBNull.Value;
                    row[nameof(PSSBranchOfficersUploadBatchItemsStaging.OfficerCommand) + "_Id"] = DBNull.Value;
                    row[nameof(PSSBranchOfficersUploadBatchItemsStaging.Rank) + "_Id"] = DBNull.Value;
                    row[nameof(PSSBranchOfficersUploadBatchItemsStaging.Gender)] = DBNull.Value;
                    row[nameof(PSSBranchOfficersUploadBatchItemsStaging.OfficerName)] = DBNull.Value;
                    row[nameof(PSSBranchOfficersUploadBatchItemsStaging.BankCode)] = DBNull.Value;
                    row[nameof(PSSBranchOfficersUploadBatchItemsStaging.AccountNumber)] = DBNull.Value;
                    row[nameof(PSSBranchOfficersUploadBatchItemsStaging.PhoneNumber)] = DBNull.Value;
                    row[nameof(PSSBranchOfficersUploadBatchItemsStaging.BankName)] = DBNull.Value;
                    row[nameof(PSSBranchOfficersUploadBatchItemsStaging.RankName)] = DBNull.Value;
                    row[nameof(PSSBranchOfficersUploadBatchItemsStaging.RankCode)] = DBNull.Value;
                    row[nameof(PSSBranchOfficersUploadBatchItemsStaging.OfficerCommandValue)] = DBNull.Value;
                }
                else
                {
                    row[nameof(PSSBranchOfficersUploadBatchItemsStaging.IPPISNumber)] = item.PersonnelReportRecord.IPPSNumber;
                    row[nameof(PSSBranchOfficersUploadBatchItemsStaging.OfficerCommandCode)] = item.PersonnelReportRecord.CommandCode;
                    row[nameof(PSSBranchOfficersUploadBatchItemsStaging.OfficerCommand) + "_Id"] = item.PoliceOfficerLogModel.Command.Id;
                    row[nameof(PSSBranchOfficersUploadBatchItemsStaging.Rank) + "_Id"] = item.PoliceOfficerLogModel.Rank.Id;
                    row[nameof(PSSBranchOfficersUploadBatchItemsStaging.Gender)] = item.PersonnelReportRecord.Gender;
                    row[nameof(PSSBranchOfficersUploadBatchItemsStaging.OfficerName)] = item.PoliceOfficerLogModel.Name;
                    row[nameof(PSSBranchOfficersUploadBatchItemsStaging.BankCode)] = item.PersonnelReportRecord.BankCode;
                    row[nameof(PSSBranchOfficersUploadBatchItemsStaging.AccountNumber)] = item.PersonnelReportRecord.AccountNumber;
                    row[nameof(PSSBranchOfficersUploadBatchItemsStaging.PhoneNumber)] = item.PersonnelReportRecord.PhoneNumber;
                    row[nameof(PSSBranchOfficersUploadBatchItemsStaging.BankName)] = item.PersonnelReportRecord.BankName;
                    row[nameof(PSSBranchOfficersUploadBatchItemsStaging.RankName)] = item.PersonnelReportRecord.RankName;
                    row[nameof(PSSBranchOfficersUploadBatchItemsStaging.RankCode)] = item.PersonnelReportRecord.RankCode;
                    row[nameof(PSSBranchOfficersUploadBatchItemsStaging.OfficerCommandValue)] = item.PersonnelReportRecord.CommandName;
                }
                row[nameof(PSSBranchOfficersUploadBatchItemsStaging.Id)] = item.PSSBranchSubUsersUploadBatchItemsStagingId;
                row[nameof(PSSBranchOfficersUploadBatchItemsStaging.ErrorMessage)] = item.ErrorMessage;
                row[nameof(PSSBranchOfficersUploadBatchItemsStaging.HasError)] = item.HasError;
                dataTable.Rows.Add(row);
            }
        }


        /// <summary>
        /// Performs a bulk update using ADO.NET on <see cref="PSSBranchOfficersUploadBatchItemsStaging"/> which requires creating a temporary table <paramref name="tempTableName"/>
        /// </summary>
        /// <param name="stagingBatchItemsDataTable"></param>
        /// <param name="tempTableName"></param>
        /// <param name="createTempQuery"></param>
        /// <exception cref="Exception">When an error occures</exception>
        public void UpdateRecordsAfterValidation(DataTable stagingBatchItemsDataTable, string tempTableName, string createTempQuery, string updateTableQuery)
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
