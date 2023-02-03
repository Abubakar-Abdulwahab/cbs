using NHibernate.Linq;
using Parkway.CBS.ClientRepository.Repositories.PAYEAPI.Contracts;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Payee;
using Parkway.CBS.Payee.PayeeAdapters.ETCC;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.ClientRepository.Repositories.PAYEAPI
{
    public class PAYEAPIBatchItemsRefDAOManager : Repository<PAYEAPIBatchItemsRef>, IPAYEAPIBatchItemsRefDAOManager
    {
        public PAYEAPIBatchItemsRefDAOManager(IUoW uow) : base(uow)
        { }

        /// <summary>
        /// Get paginated PAYE API batch items using the batchRecordStagingId
        /// </summary>
        /// <param name="payeBatchRecordStagingId"></param>
        /// <param name="chunkSize"></param>
        /// <param name="skip"></param>
        /// <returns>ICollection<PAYEAssessmentLine></returns>
        public List<PAYEAPIBatchItemsRefVM> GetBatchItems(long payeBatchRecordStagingId, int chunkSize, int skip)
        {
            return _uow.Session.Query<PAYEAPIBatchItemsRef>()
                .Where(x => x.PAYEBatchItemsStaging.PAYEBatchRecordStaging.Id == payeBatchRecordStagingId)
                .Skip(skip).Take(chunkSize)
                .Select(item => new PAYEAPIBatchItemsRefVM
                {
                    PAYEBatchItemsStagingId = item.PAYEBatchItemsStaging.Id,
                    GrossAnnualEarning = item.PAYEBatchItemsStaging.GrossAnnual,
                    IncomeTaxPerMonth = item.PAYEBatchItemsStaging.IncomeTaxPerMonthValue,
                    Exemptions = item.PAYEBatchItemsStaging.Exemptions,
                    PayerId = item.PAYEBatchItemsStaging.PayerId,
                    Month = item.PAYEBatchItemsStaging.Month,
                    Year = item.PAYEBatchItemsStaging.Year,
                    Mac = item.Mac
                }).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="payeBatchRecordStagingId"></param>
        /// <param name="payees"></param>
        /// <param name="batchLimit"></param>
        public void SaveRecords(long payeBatchRecordStagingId, List<PAYEAPIBatchItemsRefVM> payees, int batchLimit)
        {
            int chunkSize = batchLimit;
            var dataSize = payees.Count();

            double pageSize = ((double)dataSize / (double)chunkSize);
            int pages = 0;

            if (pageSize < 1 && dataSize >= 1) { pages = 1; }
            else { pages = (int)Math.Ceiling(pageSize); }
            int stopper = 0;
            int skip = 0;

            List<DataTable> listOfDataTables = new List<DataTable> { };
            try
            {
                while (stopper < pages)
                {
                    var dataTable = new DataTable("Parkway_CBS_Core_" + typeof(PAYEBatchItemsStagingValidation).Name);
                    dataTable.Columns.Add(new DataColumn("PAYEBatchRecordStaging_Id", typeof(Int64)));
                    dataTable.Columns.Add(new DataColumn("PAYEBatchItemsStaging_Id", typeof(Int64)));
                    dataTable.Columns.Add(new DataColumn("ErrorMessages", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("CreatedAtUtc", typeof(DateTime)));
                    dataTable.Columns.Add(new DataColumn("UpdatedAtUtc", typeof(DateTime)));

                    payees.Skip(skip).Take(chunkSize).ToList().ForEach(x =>
                    {
                        var row = dataTable.NewRow();
                        row["PAYEBatchRecordStaging_Id"] = payeBatchRecordStagingId;
                        row["PAYEBatchItemsStaging_Id"] = x.PAYEBatchItemsStagingId;
                        row["ErrorMessages"] = x.ErrorMessages;
                        row["CreatedAtUtc"] = DateTime.Now.ToLocalTime();
                        row["UpdatedAtUtc"] = DateTime.Now.ToLocalTime();
                        dataTable.Rows.Add(row);
                    });
                    listOfDataTables.Add(dataTable);
                    skip += chunkSize;
                    stopper++;
                }
                //we now have a collection of datatables, lets save the bunch together
                if (!SaveBundle(listOfDataTables, "Parkway_CBS_Core_" + typeof(PAYEBatchItemsStagingValidation).Name))
                { throw new Exception("Error saving payees validated item for batch staging Id " + payeBatchRecordStagingId); }
            }
            catch (Exception exception)
            {
                throw;
            }
        }

        /// <summary>
        /// populate the batch items staging table items that belong to the batch Id 
        /// with the tax entity that correspond to the payer Id on both tables 
        /// </summary>
        /// <param name="batchId"></param>
        public void PopulateTaxEntityId(long batchId)
        {
            try
            {
                var queryText = $"UPDATE its SET its.TaxEntity_Id = t.Id FROM Parkway_CBS_Core_PAYEBatchItemsStaging its INNER JOIN Parkway_CBS_Core_TaxEntity as t ON t.PayerId = its.PayerId WHERE its.PAYEBatchRecordStaging_Id = :batch_Id AND its.HasErrors = :bolVal";
                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("batch_Id", batchId);
                query.SetParameter("bolVal", false);

                query.ExecuteUpdate();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// When the batch items have been updated with the tax entity Id, 
        /// we need to update the items that don't have a tax entity Id
        /// that means a corresponding value was not found for the payer id
        /// Here we need to set the has errors to true of false
        /// </summary>
        /// <param name="batchId"></param>
        public void SetHasErrorsForNullTaxEntity(long batchId)
        {
            try
            {
                var queryText = $"UPDATE its SET its.HasErrors = :bolVal, its.ErrorMessages = :errorMessage FROM Parkway_CBS_Core_PAYEBatchItemsStaging its WHERE its.PAYEBatchRecordStaging_Id = :batch_Id AND its.TaxEntity_Id IS NULL";
                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("batch_Id", batchId);
                query.SetParameter("bolVal", true);
                query.SetParameter("errorMessage", $"Could not find user with payer Id.");

                query.ExecuteUpdate();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Update the error message for items that has HasError flag as true i.e has an existing error message.
        /// This will just append the validation error to the existing error message
        /// </summary>
        /// <param name="batchId"></param>
        public void UpdateErrorMessageAfterItemValidation(long batchId)
        {
            try
            {
                var queryText = $"UPDATE its SET its.HasErrors = :bolVal, its.ErrorMessages += '| '+itsv.ErrorMessages FROM Parkway_CBS_Core_PAYEBatchItemsStaging its" +
                    $" INNER JOIN Parkway_CBS_Core_PAYEBatchItemsStagingValidation as itsv ON itsv.PAYEBatchItemsStaging_Id = its.Id WHERE its.PAYEBatchRecordStaging_Id = :batch_Id" +
                    $" AND its.HasErrors = :hasErrorBoolVal";
                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("batch_Id", batchId);
                query.SetParameter("bolVal", true);
                query.SetParameter("hasErrorBoolVal", true);
                query.ExecuteUpdate();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Update the error message for items that has HasError flag as false i.e has no existing error message.
        /// This will just set the validation error message
        /// </summary>
        /// <param name="batchId"></param>
        public void SetHasErrorsAfterItemValidation(long batchId)
        {
            try
            {
                var queryText = $"UPDATE its SET its.HasErrors = :bolVal, its.ErrorMessages += itsv.ErrorMessages FROM Parkway_CBS_Core_PAYEBatchItemsStaging its" +
                    $" INNER JOIN Parkway_CBS_Core_PAYEBatchItemsStagingValidation as itsv ON itsv.PAYEBatchItemsStaging_Id = its.Id WHERE its.PAYEBatchRecordStaging_Id = :batch_Id" +
                    $" AND its.HasErrors = :hasErrorBoolVal";
                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("batch_Id", batchId);
                query.SetParameter("bolVal", true);
                query.SetParameter("hasErrorBoolVal", false);
                query.ExecuteUpdate();
            }
            catch (Exception)
            {
                throw;
            }
        }


    }
}
