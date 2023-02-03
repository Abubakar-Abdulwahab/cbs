using Parkway.CBS.Core.Models;
using System;
using Parkway.CBS.Core.Services.Contracts;
using Orchard.Data;
using Orchard;
using Orchard.Users.Models;
using Orchard.Logging;
using NHibernate.Criterion;
using System.Collections.Generic;
using NHibernate.Transform;
using Parkway.CBS.Core.HelperModels;
using NHibernate.Linq;
using System.Linq;
using System.Dynamic;
using Parkway.CBS.Core.Exceptions;

namespace Parkway.CBS.Core.Services
{
    public class PAYEBatchRecordStagingManager : BaseManager<PAYEBatchRecordStaging>, IPAYEBatchRecordStagingManager<PAYEBatchRecordStaging>
    {
        private readonly IRepository<PAYEBatchRecordStaging> _repository;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public PAYEBatchRecordStagingManager(IRepository<PAYEBatchRecordStaging> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _orchardServices = orchardServices;
            _transactionManager = orchardServices.TransactionManager;
            _repository = repository;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// Get PAYEBatchRecordStaging with Id and taxEntityId
        /// </summary>
        /// <param name="id"></param>
        /// <param name="taxEntityId"></param>
        /// <returns>IEnumerable{PAYEBatchRecordStaging}</returns>
        public IEnumerable<PAYEBatchRecordStaging> GetBatchStagingId(long id, long taxEntityId)
        {
            return _transactionManager.GetSession()
                                  .CreateCriteria<PAYEBatchRecordStaging>(typeof(PAYEBatchRecordStaging).Name)
                                  .CreateAlias("PAYEBatchRecordStaging.TaxEntity", "TaxEntity")
                                  .Add(Restrictions.Eq("Id", id))
                                  .Add(Restrictions.Eq("PAYEBatchRecordStaging.TaxEntity.Id", taxEntityId))
                                  .SetProjection(
                                  Projections.ProjectionList()
                                      .Add(Projections.Property<PAYEBatchRecordStaging>(x => (x.Id)), "Id")
                              ).SetResultTransformer(Transformers.AliasToBean<PAYEBatchRecordStaging>()).Future<PAYEBatchRecordStaging>();
        }



        /// <summary>
        /// Move PAYE staging to main table
        /// </summary>
        /// <param name="stagingBatchId"></param>
        /// <param name="invoiceId"></param>
        /// <param name="surcharge">Revenue head surcharge</param>
        public string InvoiceConfirmedMovePAYE(long stagingBatchId, long invoiceId, decimal surcharge)
        {
            //we need to move the staging batch record to batch record
            try
            {
                dynamic result = BaseQueryStringForMovingBatchRecordStagingToMain(stagingBatchId, surcharge);
                Int64 insertedBatchId = result.Id;

                MovePAYEBatchItemsQueryString(stagingBatchId, insertedBatchId);

                if(invoiceId > 0)
                {
                    SaveBatchRecordInvoice(insertedBatchId, invoiceId);
                }
                return (string)result.BatchRef;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                RollBackAllTransactions();
                throw;
            }
        }

        /// <summary>
        /// Attach batch record with specified batchRecordId to invoice with specified invoiceId. This method should be used when generating an invoice for an existing batch.
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="invoiceId"></param>
        public void InvoiceConfirmedAttachExisitngBatch(long batchRecordId, long invoiceId)
        {
            try
            {
                //we only save to batch record invoice table because we are essentially attaching the newly generated invoice to an existing batch
                if (invoiceId > 0)
                {
                    SaveBatchRecordInvoice(batchRecordId, invoiceId);
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                RollBackAllTransactions();
                throw;
            }
        }


        /// <summary>
        /// if there is already an invoice assigned to this PAYE batch
        /// we will need to insert an entry into the joiner table
        /// </summary>
        /// <param name="insertedBatchId"></param>
        /// <param name="invoiceId"></param>
        private void SaveBatchRecordInvoice(Int64 insertedBatchId, Int64 invoiceId)
        {
            string queryText = $"INSERT INTO Parkway_CBS_Core_PAYEBatchRecordInvoice (PAYEBatchRecord_Id, Invoice_Id, CreatedAtUtc, UpdatedAtUtc) VALUES (:insertedBatchId, :invoiceId, :dateSaved, :dateSaved)";

            var query3 = _transactionManager.GetSession().CreateSQLQuery(queryText);
            query3.SetParameter("insertedBatchId", insertedBatchId);
            query3.SetParameter("invoiceId", invoiceId);
            query3.SetParameter("dateSaved", DateTime.Now.ToLocalTime());
            query3.ExecuteUpdate();
        }


        /// <summary>
        /// Return the base query string for moving staging batch record data to batch record table
        /// </summary>
        /// <param name="stagingBatchId"></param>
        /// <param name="surcharge">Surcharge on revenue head</param>
        /// <returns>string</returns>
        private dynamic BaseQueryStringForMovingBatchRecordStagingToMain(Int64 stagingBatchId, decimal surcharge)
        {
            string queryString = $"MERGE INTO Parkway_CBS_Core_PAYEBatchRecord pdbr USING Parkway_CBS_Core_PAYEBatchRecordStaging spbr ON (spbr.Id = 0) WHEN NOT MATCHED BY TARGET AND spbr.Id = :batchId THEN INSERT (TaxEntity_Id, Billing_Id, RevenueHead_Id, FilePath, FileName, AdapterValue, CBSUser_Id, AssessmentType, OriginIdentifier, TaxPayerCode, PaymentTypeCode, DuplicateComposite, RevenueHeadSurCharge, CreatedAtUtc, UpdatedAtUtc) VALUES(spbr.TaxEntity_Id, spbr.Billing_Id, spbr.RevenueHead_Id, spbr.FilePath, spbr.FileName, spbr.AdapterValue, spbr.CBSUser_Id, spbr.AssessmentType, spbr.OriginIdentifier, spbr.TaxPayerCode, spbr.PaymentTypeCode, spbr.DuplicateComposite, :surcharge, spbr.CreatedAtUtc, spbr.UpdatedAtUtc) OUTPUT inserted.Id as InsertedBatchId, inserted.BatchRef as BatchRef;";

            var query = _transactionManager.GetSession().CreateSQLQuery(queryString);
            query.SetParameter("batchId", stagingBatchId);
            query.SetParameter("surcharge", surcharge);

            var queryResult = query.UniqueResult() as IEnumerable<object>;
            if (queryResult == null) { throw new CouldNotSaveRecord("Could not save batch record for staging batch Id " + stagingBatchId); }

            dynamic returnedResult = new ExpandoObject();
            returnedResult.Id = (Int64)queryResult.ElementAt(0);
            returnedResult.BatchRef = (string)queryResult.ElementAt(1);

            return returnedResult;
        }



        /// <summary>
        /// Move the PAYE batch staging items to the batch items table when the schedule has been confirmed 
        /// </summary>
        /// <param name="stagingBatchId"></param>
        private void MovePAYEBatchItemsQueryString(Int64 stagingBatchId, Int64 insertedBatchRecordId)
        {
            string queryText = $"INSERT INTO Parkway_CBS_Core_PAYEBatchItems (GrossAnnual, Exemptions, IncomeTaxPerMonth, TaxEntity_Id, PAYEBatchRecord_Id, Month, Year, AssessmentDate, CreatedAtUtc, UpdatedAtUtc) SELECT  bri.GrossAnnualValue, bri.ExemptionsValue, bri.IncomeTaxPerMonthValue, bri.TaxEntity_Id, :insertedBatchRecordId, bri.MonthValue, bri.YearValue, bri.AssessmentDate, bri.CreatedAtUtc, bri.UpdatedAtUtc FROM Parkway_CBS_Core_PAYEBatchItemsStaging as bri WHERE bri.PAYEBatchRecordStaging_Id = :batchId AND bri.HasErrors = :boolVal";

            var query2 = _transactionManager.GetSession().CreateSQLQuery(queryText);
            query2.SetParameter("batchId", stagingBatchId);
            query2.SetParameter("insertedBatchRecordId", insertedBatchRecordId);
            query2.SetParameter("boolVal", false);
            query2.ExecuteUpdate();
        }



        /// <summary>
        /// Get VM for Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>PAYEBatchRecordStagingVM</returns>
        public PAYEBatchRecordStagingVM GetVM(long id)
        {
            return _transactionManager.GetSession()
                .Query<PAYEBatchRecordStaging>()
                .Where(m => m.Id == id)
                .Select(m => new PAYEBatchRecordStagingVM { PercentageProgress = m.PercentageProgress, ErrorMessage = m.ErrorMessage, ErrorOccurred = m.ErrorOccurred }).SingleOrDefault();
        }

    }
}