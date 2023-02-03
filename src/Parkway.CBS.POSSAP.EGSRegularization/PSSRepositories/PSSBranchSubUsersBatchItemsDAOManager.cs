using Orchard.Users.Models;
using Parkway.CBS.ClientRepository;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.POSSAP.EGSRegularization.HelperModel;
using Parkway.CBS.POSSAP.EGSRegularization.PSSRepositories.Contracts;
using System;
using System.Collections.Generic;
using System.Data;

namespace Parkway.CBS.POSSAP.EGSRegularization.PSSRepositories
{
    public class PSSBranchSubUsersBatchItemsDAOManager : Repository<PSSBranchSubUsersUploadBatchItemsStaging>, IPSSBranchSubUsersBatchItemsDAOManager
    {
        public PSSBranchSubUsersBatchItemsDAOManager(IUoW uow) : base(uow)
        {

        }

        /// <summary>
        /// Save PSSBranchSubUsers line items as a bundle
        /// </summary>
        /// <param name="lineItems"></param>
        /// <param name="batchId"></param>
        public void SavePSSBranchSubUsersLineItemsRecords(List<PSSBranchSubUsersItemVM> lineItems, long batchId)
        {
            //Logger.Information("Saving PSSBranchSubUsers records for batch id " + batchId);
            //save PSSBranchSubUsers line items to table
            try
            {
                var dataTable = new DataTable("Parkway_CBS_Police_Core_" + typeof(PSSBranchSubUsersUploadBatchItemsStaging).Name);
                dataTable.Columns.Add(new DataColumn(nameof(PSSBranchSubUsersUploadBatchItemsStaging.BranchStateCode), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(PSSBranchSubUsersUploadBatchItemsStaging.BranchLGACode), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(PSSBranchSubUsersUploadBatchItemsStaging.BranchStateValue), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(PSSBranchSubUsersUploadBatchItemsStaging.BranchLGAValue), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(PSSBranchSubUsersUploadBatchItemsStaging.BranchAddress), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(PSSBranchSubUsersUploadBatchItemsStaging.BranchName), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(PSSBranchSubUsersUploadBatchItemsStaging.SubUserName), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(PSSBranchSubUsersUploadBatchItemsStaging.SubUserEmail), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(PSSBranchSubUsersUploadBatchItemsStaging.SubUserPhoneNumber), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(PSSBranchSubUsersUploadBatchItemsStaging.HasError), typeof(bool)));
                dataTable.Columns.Add(new DataColumn(nameof(PSSBranchSubUsersUploadBatchItemsStaging.ErrorMessage), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(PSSBranchSubUsersUploadBatchItemsStaging.PSSBranchSubUsersUploadBatchStaging) + "_Id", typeof(long)));
                dataTable.Columns.Add(new DataColumn(nameof(PSSBranchSubUsersUploadBatchItemsStaging.CreatedAtUtc), typeof(DateTime)));
                dataTable.Columns.Add(new DataColumn(nameof(PSSBranchSubUsersUploadBatchItemsStaging.UpdatedAtUtc), typeof(DateTime)));

                foreach (var lineItem in lineItems)
                {
                    DataRow row = dataTable.NewRow();
                    row[nameof(PSSBranchSubUsersUploadBatchItemsStaging.BranchStateCode)] = lineItem.BranchStateCode;
                    row[nameof(PSSBranchSubUsersUploadBatchItemsStaging.BranchLGACode)] = lineItem.BranchLGACode;
                    row[nameof(PSSBranchSubUsersUploadBatchItemsStaging.BranchStateValue)] = lineItem.BranchStateValue;
                    row[nameof(PSSBranchSubUsersUploadBatchItemsStaging.BranchLGAValue)] = lineItem.BranchLGAValue;
                    row[nameof(PSSBranchSubUsersUploadBatchItemsStaging.BranchAddress)] = lineItem.BranchAddress;
                    row[nameof(PSSBranchSubUsersUploadBatchItemsStaging.BranchName)] = lineItem.BranchName;
                    row[nameof(PSSBranchSubUsersUploadBatchItemsStaging.SubUserName)] = lineItem.SubUserName;
                    row[nameof(PSSBranchSubUsersUploadBatchItemsStaging.SubUserEmail)] = lineItem.SubUserEmail;
                    row[nameof(PSSBranchSubUsersUploadBatchItemsStaging.SubUserPhoneNumber)] = lineItem.SubUserPhoneNumber;
                    row[nameof(PSSBranchSubUsersUploadBatchItemsStaging.HasError)] = lineItem.HasError;
                    row[nameof(PSSBranchSubUsersUploadBatchItemsStaging.ErrorMessage)] = lineItem.ErrorMessage;
                    row[nameof(PSSBranchSubUsersUploadBatchItemsStaging.PSSBranchSubUsersUploadBatchStaging) + "_Id"] = batchId;
                    row[nameof(PSSBranchSubUsersUploadBatchItemsStaging.CreatedAtUtc)] = DateTime.Now.ToLocalTime();
                    row[nameof(PSSBranchSubUsersUploadBatchItemsStaging.UpdatedAtUtc)] = DateTime.Now.ToLocalTime();
                    dataTable.Rows.Add(row);
                }

                if (!SaveBundle(dataTable, "Parkway_CBS_Police_Core_" + typeof(PSSBranchSubUsersUploadBatchItemsStaging).Name))
                { throw new Exception("Error saving PSSBranchSubUsers schedule excel file details for batch Id " + batchId); }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Resolves Branch LGA Ids and Branch LGA values for PSSBranchSubUsers items that have been created in batch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        public void ResolveBranchLGAAndStateIdsAndValuesForCreatedBranches(long batchId)
        {
            try
            {
                var queryText = $"UPDATE T1 SET " +
                                    $"T1.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.BranchLGA)}_Id = T2.{nameof(LGA.Id)}, T1.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.BranchLGAValue)} = T2.{nameof(LGA.Name)}, " +
                                    $"T1.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.BranchState)}_Id = T3.{nameof(StateModel.Id)}, T1.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.BranchStateValue)} = T3.{nameof(StateModel.Name)} " +
                                    $"FROM Parkway_CBS_Police_Core_{nameof(PSSBranchSubUsersUploadBatchItemsStaging)} AS T1 INNER JOIN Parkway_CBS_Core_{nameof(LGA)} AS T2 " +
                                    $"ON T1.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.BranchLGACode)} = T2.{nameof(LGA.CodeName)} " +
                                    $"INNER JOIN Parkway_CBS_Core_{nameof(StateModel)} AS T3 " +
                                    $"ON T1.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.BranchStateCode)} = T3.{nameof(StateModel.ShortName)} WHERE " +
                                    $"T1.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.PSSBranchSubUsersUploadBatchStaging)}_Id = :batchId AND " +
                                    $"T2.{nameof(LGA.State)}_Id = T3.{nameof(StateModel.Id)} AND " +
                                    $"T1.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.HasError)} = :hasError;";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("hasError", false);
                query.SetParameter("batchId", batchId);
                query.ExecuteUpdate();
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Perform bulk update on PSSBranchSubUsersItems, updating hasError and errorMessage for items in batch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="errorMessage"></param>
        public void UpdateBranchLGAAndStateResolutionErrorMessage(long batchId, string errorMessage)
        {
            try
            {
                var queryText = $"UPDATE Parkway_CBS_Police_Core_{nameof(PSSBranchSubUsersUploadBatchItemsStaging)} " +
                $"SET {nameof(PSSBranchSubUsersUploadBatchItemsStaging.HasError)} = 1, {nameof(PSSBranchSubUsersUploadBatchItemsStaging.ErrorMessage)} = " +
                $"CONCAT({nameof(PSSBranchSubUsersUploadBatchItemsStaging.ErrorMessage)}, :errorMessage) WHERE {nameof(PSSBranchSubUsersUploadBatchItemsStaging.PSSBranchSubUsersUploadBatchStaging)}_Id = :batch_Id " +
                $"AND {nameof(PSSBranchSubUsersUploadBatchItemsStaging.HasError)} = :hasError " +
                $"AND ({nameof(PSSBranchSubUsersUploadBatchItemsStaging.BranchLGAValue)} is null) " +
                $"AND ({nameof(PSSBranchSubUsersUploadBatchItemsStaging.BranchStateValue)} is null);";
                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("batch_Id", batchId);
                query.SetParameter("hasError", false);
                query.SetParameter("errorMessage", errorMessage);
                query.ExecuteUpdate();
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Perform bulk validation and update on PSSBranchSubUsersItems SubUserEmail column, updating hasError and errorMessage for items in batch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="errorMessage"></param>
        public void ValidateSubUserEmailIsNotExisitingAndUpdatePSSBranchSubUsersItemErrorMessage(long batchId, string errorMessage)
        {
            try
            {
                var queryText = $"UPDATE STAGING SET staging.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.HasError)} = 1, " +
                                $"STAGING.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.ErrorMessage)} = CONCAT({nameof(PSSBranchSubUsersUploadBatchItemsStaging.ErrorMessage)}, :errorMessage) " +
                                $"FROM Parkway_CBS_Police_Core_{nameof(PSSBranchSubUsersUploadBatchItemsStaging)} STAGING " +
                                $"INNER JOIN Orchard_Users_{nameof(UserPartRecord)} UPR ON UPR.{nameof(UserPartRecord.Email)} = STAGING.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.SubUserEmail)} " +
                                $"WHERE STAGING.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.PSSBranchSubUsersUploadBatchStaging)}_Id = :batchId " +
                                $"AND {nameof(PSSBranchSubUsersUploadBatchItemsStaging.HasError)} = :hasError; ";
                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("batchId", batchId);
                query.SetParameter("hasError", false);
                query.SetParameter("errorMessage", errorMessage);
                query.ExecuteUpdate();
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Perform validation on PSSBranchSubUsersItems SubUserEmail column that duplicates are not uploaded, updating hasError and errorMessage for items in batch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="errorMessage"></param>
        public void ValidateSubUserEmailIsNotDuplicateAndUpdatePSSBranchSubUsersItemErrorMessage(long batchId, string errorMessage)
        {
            try
            {
                var queryText = $"UPDATE S1 SET S1.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.HasError)} = 1, " +
                                $"S1.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.ErrorMessage)} = CONCAT(S1.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.ErrorMessage)}, :errorMessage) " +
                                $"FROM Parkway_CBS_Police_Core_{nameof(PSSBranchSubUsersUploadBatchItemsStaging)} AS S1 " +
                                $"WHERE (SELECT COUNT(S2.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.SubUserEmail)}) " +
                                $"FROM Parkway_CBS_Police_Core_{nameof(PSSBranchSubUsersUploadBatchItemsStaging)} AS S2 " +
                                $"GROUP BY S2.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.PSSBranchSubUsersUploadBatchStaging)}_Id, S2.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.SubUserEmail)} " +
                                $"HAVING S1.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.SubUserEmail)} = S2.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.SubUserEmail)} " +
                                $"AND S2.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.PSSBranchSubUsersUploadBatchStaging)}_Id = :batchId) > 1 " +
                                $"AND S1.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.PSSBranchSubUsersUploadBatchStaging)}_Id = :batchId " +
                                $"AND S1.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.HasError)} = :hasError; ";
                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("batchId", batchId);
                query.SetParameter("hasError", false);
                query.SetParameter("errorMessage", errorMessage);
                query.ExecuteUpdate();
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Perform validation on PSSBranchSubUsersItems phoneNumber column that a user with same number doesn't exist on CBSUser, updating hasError and errorMessage for items in batch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="errorMessage"></param>
        public void ValidatePhoneNumberIsNotExisitingAndUpdatePSSBranchSubUsersItemErrorMessage(long batchId, string errorMessage)
        {
            try
            {
                var queryText = $"UPDATE STAGING SET STAGING.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.HasError)} = 1, " +
                                $"STAGING.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.ErrorMessage)} = CONCAT({nameof(PSSBranchSubUsersUploadBatchItemsStaging.ErrorMessage)}, :errorMessage) " +
                                $"FROM Parkway_CBS_Police_Core_{nameof(PSSBranchSubUsersUploadBatchItemsStaging)} STAGING " +
                                $"INNER JOIN Parkway_CBS_Core_{nameof(CBSUser)} CBSUSER ON CBSUSER.{nameof(CBSUser.PhoneNumber)} = STAGING.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.SubUserPhoneNumber)} " +
                                $"WHERE STAGING.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.PSSBranchSubUsersUploadBatchStaging)}_Id = :batchId " +
                                $"AND STAGING.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.HasError)} = :hasError; ";
                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("batchId", batchId);
                query.SetParameter("hasError", false);
                query.SetParameter("errorMessage", errorMessage);
                query.ExecuteUpdate();
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Perform validation on PSSBranchSubUsersItems phoneNumber column that duplicates are not uploaded, updating hasError and errorMessage for items in batch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="errorMessage"></param>
        public void ValidatePhoneNumberIsNotDuplicateAndUpdatePSSBranchSubUsersItemErrorMessage(long batchId, string errorMessage)
        {
            try
            {
                var queryText = $"UPDATE S1 SET S1.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.HasError)} = 1, " +
                                $"S1.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.ErrorMessage)} = CONCAT(S1.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.ErrorMessage)}, :errorMessage) " +
                                $"FROM Parkway_CBS_Police_Core_{nameof(PSSBranchSubUsersUploadBatchItemsStaging)} AS S1 " +
                                $"WHERE (SELECT COUNT(S2.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.SubUserPhoneNumber)}) " +
                                $"FROM Parkway_CBS_Police_Core_{nameof(PSSBranchSubUsersUploadBatchItemsStaging)} AS S2 " +
                                $"GROUP BY S2.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.PSSBranchSubUsersUploadBatchStaging)}_Id, S2.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.SubUserPhoneNumber)} " +
                                $"HAVING S1.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.SubUserPhoneNumber)} = S2.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.SubUserPhoneNumber)} " +
                                $"AND S2.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.PSSBranchSubUsersUploadBatchStaging)}_Id = :batchId) > 1 " +
                                $"AND S1.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.PSSBranchSubUsersUploadBatchStaging)}_Id = :batchId " +
                                $"AND S1.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.HasError)} = :hasError; ";
                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("batchId", batchId);
                query.SetParameter("hasError", false);
                query.SetParameter("errorMessage", errorMessage);
                query.ExecuteUpdate();
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Perform validation on PSSBranchSubUsersItems branch address column that a tex entity with same address doesn't exist on TaxEntityProfileLocation, updating hasError and errorMessage for items in batch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="errorMessage"></param>
        public void ValidateAddressIsNotExisitingAndUpdatePSSBranchSubUsersItemErrorMessage(long batchId, string errorMessage)
        {
            try
            {
                var queryText = $"UPDATE ITEMS SET ITEMS.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.HasError)} = 1, " +
                                $"ITEMS.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.ErrorMessage)} = CONCAT(ITEMS.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.ErrorMessage)}, :errorMessage) " +
                                $"FROM Parkway_CBS_Police_Core_{nameof(PSSBranchSubUsersUploadBatchItemsStaging)} AS ITEMS " +
                                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(PSSBranchSubUsersUploadBatchStaging)} AS BATCH " +
                                $"ON ITEMS.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.PSSBranchSubUsersUploadBatchStaging)}_Id = BATCH.{nameof(PSSBranchSubUsersUploadBatchStaging.Id)} " +
                                $"INNER JOIN Parkway_CBS_Core_{nameof(TaxEntityProfileLocation)} AS LOC " +
                                $"ON BATCH.{nameof(PSSBranchSubUsersUploadBatchStaging.TaxEntity)}_Id = LOC.{nameof(TaxEntityProfileLocation.TaxEntity)}_Id " +
                                $"AND LOC.{nameof(TaxEntityProfileLocation.Address)} = ITEMS.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.BranchAddress)} " +
                                $"AND ITEMS.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.PSSBranchSubUsersUploadBatchStaging)}_Id = :batchId " +
                                $"AND ITEMS.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.HasError)} = :hasError; ";
                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("batchId", batchId);
                query.SetParameter("hasError", false);
                query.SetParameter("errorMessage", errorMessage);
                query.ExecuteUpdate();
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Perform validation on PSSBranchSubUsersItems branch Address column that duplicates are not uploaded, updating hasError and errorMessage for items in batch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="errorMessage"></param>
        public void ValidateAddressIsNotDuplicateAndUpdatePSSBranchSubUsersItemErrorMessage(long batchId, string errorMessage)
        {
            try
            {
                var queryText = $"UPDATE S1 SET S1.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.HasError)} = 1, " +
                                $"S1.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.ErrorMessage)} = CONCAT(S1.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.ErrorMessage)}, :errorMessage) " +
                                $"FROM Parkway_CBS_Police_Core_{nameof(PSSBranchSubUsersUploadBatchItemsStaging)} AS S1 " +
                                $"WHERE (SELECT COUNT(S2.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.BranchAddress)}) " +
                                $"FROM Parkway_CBS_Police_Core_{nameof(PSSBranchSubUsersUploadBatchItemsStaging)} AS S2 " +
                                $"GROUP BY S2.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.PSSBranchSubUsersUploadBatchStaging)}_Id, S2.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.BranchAddress)} " +
                                $"HAVING S1.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.BranchAddress)} = S2.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.BranchAddress)} " +
                                $"AND S2.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.PSSBranchSubUsersUploadBatchStaging)}_Id = :batchId) > 1 " +
                                $"AND S1.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.PSSBranchSubUsersUploadBatchStaging)}_Id = :batchId " +
                                $"AND S1.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.HasError)} = :hasError; ";
                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("batchId", batchId);
                query.SetParameter("hasError", false);
                query.SetParameter("errorMessage", errorMessage);
                query.ExecuteUpdate();
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Perform validation on PSSBranchSubUsersItems branch name column that a tex entity with same name doesn't exist on TaxEntityProfileLocation, updating hasError and errorMessage for items in batch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="errorMessage"></param>
        public void ValidateBranchNameIsNotExisitingAndUpdatePSSBranchSubUsersItemErrorMessage(long batchId, string errorMessage)
        {
            try
            {
                var queryText = $"UPDATE ITEMS SET ITEMS.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.HasError)} = 1, " +
                                $"ITEMS.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.ErrorMessage)} = CONCAT(ITEMS.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.ErrorMessage)}, :errorMessage) " +
                                $"FROM Parkway_CBS_Police_Core_{nameof(PSSBranchSubUsersUploadBatchItemsStaging)} AS ITEMS " +
                                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(PSSBranchSubUsersUploadBatchStaging)} AS BATCH " +
                                $"ON ITEMS.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.PSSBranchSubUsersUploadBatchStaging)}_Id = BATCH.{nameof(PSSBranchSubUsersUploadBatchStaging.Id)} " +
                                $"INNER JOIN Parkway_CBS_Core_{nameof(TaxEntityProfileLocation)} AS LOC " +
                                $"ON BATCH.{nameof(PSSBranchSubUsersUploadBatchStaging.TaxEntity)}_Id = LOC.{nameof(TaxEntityProfileLocation.TaxEntity)}_Id " +
                                $"AND LOC.{nameof(TaxEntityProfileLocation.Name)} = ITEMS.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.BranchName)} " +
                                $"AND ITEMS.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.PSSBranchSubUsersUploadBatchStaging)}_Id = :batchId " +
                                $"AND ITEMS.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.HasError)} = :hasError; ";
                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("batchId", batchId);
                query.SetParameter("hasError", false);
                query.SetParameter("errorMessage", errorMessage);
                query.ExecuteUpdate();
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Perform validation on PSSBranchSubUsersItems branch name column that duplicates are not uploaded, updating hasError and errorMessage for items in batch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="errorMessage"></param>
        public void ValidateBranchNameIsNotDuplicateAndUpdatePSSBranchSubUsersItemErrorMessage(long batchId, string errorMessage)
        {
            try
            {
                var queryText = $"UPDATE S1 SET S1.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.HasError)} = 1, " +
                                $"S1.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.ErrorMessage)} = CONCAT(S1.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.ErrorMessage)}, :errorMessage) " +
                                $"FROM Parkway_CBS_Police_Core_{nameof(PSSBranchSubUsersUploadBatchItemsStaging)} AS S1 " +
                                $"WHERE (SELECT COUNT(S2.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.BranchName)}) " +
                                $"FROM Parkway_CBS_Police_Core_{nameof(PSSBranchSubUsersUploadBatchItemsStaging)} AS S2 " +
                                $"GROUP BY S2.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.PSSBranchSubUsersUploadBatchStaging)}_Id, S2.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.BranchName)} " +
                                $"HAVING S1.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.BranchName)} = S2.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.BranchName)} " +
                                $"AND S2.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.PSSBranchSubUsersUploadBatchStaging)}_Id = :batchId) > 1 " +
                                $"AND S1.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.PSSBranchSubUsersUploadBatchStaging)}_Id = :batchId " +
                                $"AND S1.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.HasError)} = :hasError; ";
                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("batchId", batchId);
                query.SetParameter("hasError", false);
                query.SetParameter("errorMessage", errorMessage);
                query.ExecuteUpdate();
            }
            catch (Exception) { throw; }
        }
    }
}
