using NHibernate.Linq;
using Parkway.CBS.ClientRepository;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.POSSAP.EGSRegularization.HelperModels;
using Parkway.CBS.POSSAP.EGSRegularization.PSSRepositories.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Parkway.CBS.POSSAP.EGSRegularization.PSSRepositories
{
    public class GenerateRequestWithoutOfficersUploadBatchItemsDAOManager : Repository<GenerateRequestWithoutOfficersUploadBatchItemsStaging>, IGenerateRequestWithoutOfficersUploadBatchItemsDAOManager
    {

        public GenerateRequestWithoutOfficersUploadBatchItemsDAOManager(IUoW uow) : base(uow)
        {

        }

        /// <summary>
        /// Save GenerateRequestWithoutOfficers line items as a bundle
        /// </summary>
        /// <param name="lineItems"></param>
        /// <param name="batchId"></param>
        public void SaveGenerateRequestWithoutOfficersLineItemsRecords(List<GenerateRequestWithoutOfficersUploadItemVM> lineItems, long batchId)
        {
            //Logger.Information("Saving GenerateRequestWithoutOfficersUpload records for batch id " + batchId);
            //save GenerateRequestWithoutOfficersUpload line items to table
            try
            {
                var dataTable = new DataTable("Parkway_CBS_Police_Core_" + typeof(GenerateRequestWithoutOfficersUploadBatchItemsStaging).Name);
                dataTable.Columns.Add(new DataColumn(nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.BranchCode), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.NumberOfOfficers), typeof(int)));
                dataTable.Columns.Add(new DataColumn(nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.NumberOfOfficersValue), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.CommandCode), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.CommandType), typeof(int)));
                dataTable.Columns.Add(new DataColumn(nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.DayType), typeof(int)));
                dataTable.Columns.Add(new DataColumn(nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.CommandTypeValue), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.DayTypeValue), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.HasError), typeof(bool)));
                dataTable.Columns.Add(new DataColumn(nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.ErrorMessage), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.GenerateRequestWithoutOfficersUploadBatchStaging) + "_Id", typeof(long)));
                dataTable.Columns.Add(new DataColumn(nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.CreatedAtUtc), typeof(DateTime)));
                dataTable.Columns.Add(new DataColumn(nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.UpdatedAtUtc), typeof(DateTime)));

                foreach (var lineItem in lineItems)
                {
                    DataRow row = dataTable.NewRow();
                    row[nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.BranchCode)] = lineItem.BranchCode;
                    row[nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.NumberOfOfficers)] = lineItem.NumberOfOfficers;
                    row[nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.NumberOfOfficersValue)] = lineItem.NumberOfOfficersValue;
                    row[nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.CommandCode)] = lineItem.CommandCode;
                    row[nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.CommandType)] = lineItem.CommandType;
                    row[nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.DayType)] = lineItem.DayType;
                    row[nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.CommandTypeValue)] = lineItem.CommandTypeValue;
                    row[nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.DayTypeValue)] = lineItem.DayTypeValue;
                    row[nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.HasError)] = lineItem.HasError;
                    row[nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.ErrorMessage)] = lineItem.ErrorMessage;
                    row[nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.GenerateRequestWithoutOfficersUploadBatchStaging) + "_Id"] = batchId;
                    row[nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.CreatedAtUtc)] = DateTime.Now.ToLocalTime();
                    row[nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.UpdatedAtUtc)] = DateTime.Now.ToLocalTime();
                    dataTable.Rows.Add(row);
                }

                if (!SaveBundle(dataTable, "Parkway_CBS_Police_Core_" + typeof(GenerateRequestWithoutOfficersUploadBatchItemsStaging).Name))
                { throw new Exception("Error saving GenerateRequestWithoutOfficers schedule excel file details for batch Id " + batchId); }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Perform bulk validation on GenerateRequestWithoutOfficersItems BRANCH CODE column to make sure it is same as current batch branch, updating hasError and errorMessage for items in batch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="errorMessage"></param>
        public void ValidateBranchCodeIsSameAsCurrentBranchAndUpdateGenerateRequestWithoutOfficersItemErrorMessage(long batchId, string errorMessage)
        {
            var queryText = $"UPDATE ITEMS SET ITEMS.{nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.HasError)} = 1, " +
                            $"ITEMS.{nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.ErrorMessage)} = CONCAT(ITEMS.{nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.ErrorMessage)}, :errorMessage) " +
                            $"FROM Parkway_CBS_Police_Core_{nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging)} AS ITEMS " +
                            $"INNER JOIN Parkway_CBS_Police_Core_{nameof(GenerateRequestWithoutOfficersUploadBatchStaging)} AS BATCH " +
                            $"ON ITEMS.{nameof(GenerateRequestWithoutOfficersUploadBatchStaging)}_Id = BATCH.{nameof(GenerateRequestWithoutOfficersUploadBatchStaging.Id)} " +
                            $"INNER JOIN Parkway_CBS_Core_{nameof(TaxEntityProfileLocation)} AS LOC " +
                            $"ON BATCH.{nameof(TaxEntityProfileLocation)}_Id = LOC.{nameof(TaxEntityProfileLocation.Id)} " +
                            $"WHERE LOC.{nameof(TaxEntityProfileLocation.Code)} != ITEMS.{nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.BranchCode)} " +
                            $"AND ITEMS.{nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.GenerateRequestWithoutOfficersUploadBatchStaging)}_Id = :batchId " +
                            $"AND ITEMS.{nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.HasError)} = :hasError; ";
            var query = _uow.Session.CreateSQLQuery(queryText);
            query.SetParameter("batchId", batchId);
            query.SetParameter("hasError", false);
            query.SetParameter("errorMessage", errorMessage);
            query.ExecuteUpdate();
        }

        /// <summary>
        /// Resolves Command Ids for GenerateRequestWithoutOfficers items that have been created in batch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        public void ResolveCommandIdsForGenerateRequestsWithoutOfficersAlreadyCreated(long batchId)
        {
            try
            {
                var queryText = $"UPDATE T1 SET " +
                                    $"T1.{nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.Command)}_Id = T2.{nameof(Command.Id)} " +
                                    $"FROM Parkway_CBS_Police_Core_{nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging)} AS T1 INNER JOIN Parkway_CBS_Police_Core_{nameof(Command)} AS T2 " +
                                    $"ON T1.{nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.CommandCode)} = T2.{nameof(Command.Code)} WHERE " +
                                    $"T1.{nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.GenerateRequestWithoutOfficersUploadBatchStaging)}_Id = :batchId AND " +
                                    $"T1.{nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.HasError)} = :hasError;";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("hasError", false);
                query.SetParameter("batchId", batchId);
                query.ExecuteUpdate();
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Perform bulk update on GenerateRequestWithoutOfficers, updating hasError and errorMessage for items in batch with specified id where commandId is null
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="errorMessage"></param>
        public void UpdateCommandIdResolutionErrorMessage(long batchId, string errorMessage)
        {
            try
            {
                var queryText = $"UPDATE Parkway_CBS_Police_Core_{nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging)} " +
                $"SET {nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.HasError)} = 1, {nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.ErrorMessage)} = " +
                $"CONCAT({nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.ErrorMessage)}, :errorMessage) WHERE " +
                $"{nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.GenerateRequestWithoutOfficersUploadBatchStaging)}_Id = :batch_Id " +
                $"AND {nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.HasError)} = :hasError " +
                $"AND ({nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.Command)}_Id is null);";
                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("batch_Id", batchId);
                query.SetParameter("hasError", false);
                query.SetParameter("errorMessage", errorMessage);
                query.ExecuteUpdate();
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Perform validation on GenerateRequestWithoutOfficers items that duplicates of Command Code, Command Type and Day Type combined are not existing, updating hasError and errorMessage for items in batch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="errorMessage"></param>
        public void ValidateCommandCodeCommandTypeDayTypeCombinationIsNotDuplicateAndUpdateGenerateRequestWithoutOfficersItemErrorMessage(long batchId, string errorMessage)
        {
            try
            {
                var queryText = $"UPDATE S1 SET S1.{nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.HasError)} = 1, " +
                                $"S1.{nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.ErrorMessage)} = CONCAT(S1.{nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.ErrorMessage)}, :errorMessage) " +
                                $"FROM Parkway_CBS_Police_Core_{nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging)} AS S1 " +
                                $"WHERE (SELECT COUNT(S2.{nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.DayType)}) " +
                                $"FROM Parkway_CBS_Police_Core_{nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging)} AS S2 " +
                                $"GROUP BY S2.{nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.GenerateRequestWithoutOfficersUploadBatchStaging)}_Id, S2.{nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.CommandCode)}, S2.{nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.CommandType)}, S2.{nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.DayType)} " +
                                $"HAVING S1.{nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.CommandCode)} = S2.{nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.CommandCode)} " +
                                $"AND S1.{nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.CommandType)} = S2.{nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.CommandType)} " +
                                $"AND S1.{nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.DayType)} = S2.{nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.DayType)} " +
                                $"AND S2.{nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.GenerateRequestWithoutOfficersUploadBatchStaging)}_Id = :batchId) > 1 " +
                                $"AND S1.{nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.GenerateRequestWithoutOfficersUploadBatchStaging)}_Id = :batchId " +
                                $"AND S1.{nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.HasError)} = :hasError; ";
                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("batchId", batchId);
                query.SetParameter("hasError", false);
                query.SetParameter("errorMessage", errorMessage);
                query.ExecuteUpdate();
            }
            catch (Exception) { throw; }
        }


        /// <summary>
        /// Gets items in batch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns></returns>
        public IEnumerable<GenerateRequestWithoutOfficersUploadBatchItemsStagingDTO> GetItems(long batchId)
        {
            try
            {
                return _uow.Session.Query<GenerateRequestWithoutOfficersUploadBatchItemsStaging>().Where(x => x.GenerateRequestWithoutOfficersUploadBatchStaging.Id == batchId && !x.HasError)
                    .Select(x => new GenerateRequestWithoutOfficersUploadBatchItemsStagingDTO
                    {
                        Id = x.Id,
                        NumberOfOfficers = x.NumberOfOfficers,
                        DayType = x.DayType,
                        CommandType = x.CommandType,
                        CommandCode = x.CommandCode,
                        CommandId = x.Command.Id
                    });
            }
            catch (Exception) { throw; }
        }
    }
}
