using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.Transform;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Payee;
using Parkway.CBS.Payee.PayeeAdapters.ETCC;

namespace Parkway.CBS.Core.Services
{
    public class PAYEBatchItemsStagingManager : BaseManager<PAYEBatchItemsStaging>, IPAYEBatchItemsStagingManager<PAYEBatchItemsStaging>
    {
        private readonly IRepository<PAYEBatchItemsStaging> _repository;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public PAYEBatchItemsStagingManager(IRepository<PAYEBatchItemsStaging> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _orchardServices = orchardServices;
            _transactionManager = orchardServices.TransactionManager;
            _repository = repository;
            Logger = NullLogger.Instance;
        }

        public void SavePAYELineItemsRecords(List<PayeeAssessmentLineRecordModel> payeItems, long batchStagingId)
        {
            Logger.Information("Saving PAYE assessment records for batch id " + batchStagingId);
            //save entities into temp table
            int chunkSize = 500000;
            int pages = Util.Pages(chunkSize, payeItems.Count);
            int stopper = 0;
            int skip = 0;
            //var startTime = Stopwatch.StartNew();
            try
            {
                var dataTable = new DataTable("Parkway_CBS_Core_" + typeof(PAYEBatchItemsStaging).Name);
                dataTable.Columns.Add(new DataColumn(nameof(PAYEBatchItemsStaging.GrossAnnual), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(PAYEBatchItemsStaging.GrossAnnualValue), typeof(decimal)));
                dataTable.Columns.Add(new DataColumn(nameof(PAYEBatchItemsStaging.Exemptions), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(PAYEBatchItemsStaging.ExemptionsValue), typeof(decimal)));
                dataTable.Columns.Add(new DataColumn(nameof(PAYEBatchItemsStaging.PayerId), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(PAYEBatchItemsStaging.IncomeTaxPerMonth), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(PAYEBatchItemsStaging.IncomeTaxPerMonthValue), typeof(decimal)));
                dataTable.Columns.Add(new DataColumn(nameof(PAYEBatchItemsStaging.Month), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(PAYEBatchItemsStaging.MonthValue), typeof(int)));
                dataTable.Columns.Add(new DataColumn(nameof(PAYEBatchItemsStaging.Year), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(PAYEBatchItemsStaging.YearValue), typeof(int)));
                dataTable.Columns.Add(new DataColumn(nameof(PAYEBatchItemsStaging.PAYEBatchRecordStaging) + "_Id", typeof(Int64)));
                dataTable.Columns.Add(new DataColumn(nameof(PAYEBatchItemsStaging.HasErrors), typeof(bool)));
                dataTable.Columns.Add(new DataColumn(nameof(PAYEBatchItemsStaging.ErrorMessages), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(PAYEBatchItemsStaging.AssessmentDate), typeof(DateTime)));
                dataTable.Columns.Add(new DataColumn(nameof(PAYEBatchItemsStaging.CreatedAtUtc), typeof(DateTime)));
                dataTable.Columns.Add(new DataColumn(nameof(PAYEBatchItemsStaging.UpdatedAtUtc), typeof(DateTime)));

                while (stopper < pages)
                {
                    payeItems.Skip(skip).Take(chunkSize).ToList().ForEach(x =>
                    {
                        DataRow row = dataTable.NewRow();
                        row[nameof(PAYEBatchItemsStaging.HasErrors)] = x.HasError;
                        row[nameof(PAYEBatchItemsStaging.IncomeTaxPerMonth)] = x.PayeeBreakDown.TaxStringValue;
                        row[nameof(PAYEBatchItemsStaging.IncomeTaxPerMonthValue)] = x.PayeeBreakDown.Tax;
                        row[nameof(PAYEBatchItemsStaging.PayerId)] = x.TaxPayerId.Value;
                        row[nameof(PAYEBatchItemsStaging.Year)] = x.Year.StringValue;
                        row[nameof(PAYEBatchItemsStaging.YearValue)] = x.Year.Value;
                        row[nameof(PAYEBatchItemsStaging.Month)] = x.Month.StringValue;
                        row[nameof(PAYEBatchItemsStaging.MonthValue)] = x.Month.Value;
                        row[nameof(PAYEBatchItemsStaging.PAYEBatchRecordStaging) + "_Id"] = batchStagingId;
                        row[nameof(PAYEBatchItemsStaging.Exemptions)] = x.Exemptions.StringValue;
                        row[nameof(PAYEBatchItemsStaging.ExemptionsValue)] = x.Exemptions.Value;
                        row[nameof(PAYEBatchItemsStaging.GrossAnnual)] = x.GrossAnnualEarnings.StringValue;
                        row[nameof(PAYEBatchItemsStaging.GrossAnnualValue)] = x.GrossAnnualEarnings.Value;
                        row[nameof(PAYEBatchItemsStaging.ErrorMessages)] = x.ErrorMessages;
                        row[nameof(PAYEBatchItemsStaging.AssessmentDate)] = x.AssessmentDate.HasValue ? (object)x.AssessmentDate.Value : DBNull.Value;
                        row[nameof(PAYEBatchItemsStaging.CreatedAtUtc)] = DateTime.Now.ToLocalTime();
                        row[nameof(PAYEBatchItemsStaging.UpdatedAtUtc)] = DateTime.Now.ToLocalTime();
                        dataTable.Rows.Add(row);
                    });
                    Logger.Information(string.Format("Insertion for PAYE assessment batch items has started skip {0}", skip));

                    if (!SaveBundle(dataTable, "Parkway_CBS_Core_" + typeof(PAYEBatchItemsStaging).Name))
                    { throw new Exception("Error saving excel file details for batch Id " + batchStagingId); }

                    skip += chunkSize;
                    stopper++;
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message); throw;
            }
        }

        /// <summary>
        /// Save PAYE batch items in staging table
        /// <para>This uses ADO.Net for faster insertion for files with large amounts of data</para>
        /// </summary>
        /// <param name="payeItems"></param>
        /// <param name="batchStagingId"></param>
        public void SavePAYEAssessmentLineItems(List<PAYEAssessmentLine> payeItems, long batchStagingId)
        {
            Logger.Information("Saving PAYE assessment records for batch id " + batchStagingId);

            int chunkSize = 2000;

            string sChunkSize = AppSettingsConfigurations.GetSettingsValue(AppSettingEnum.PayeExcelSaveChunkSize);

            if (!string.IsNullOrEmpty(sChunkSize))
            {
                int.TryParse(sChunkSize, out chunkSize);
            }

            //save entities into temp table
            int pages = Util.Pages(chunkSize, payeItems.Count);
            int stopper = 0;
            int skip = 0;

            try
            {
                var dataTable = new DataTable("Parkway_CBS_Core_" + typeof(PAYEBatchItemsStaging).Name);
                dataTable.Columns.Add(new DataColumn(nameof(PAYEBatchItemsStaging.GrossAnnual), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(PAYEBatchItemsStaging.GrossAnnualValue), typeof(decimal)));
                dataTable.Columns.Add(new DataColumn(nameof(PAYEBatchItemsStaging.Exemptions), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(PAYEBatchItemsStaging.ExemptionsValue), typeof(decimal)));
                dataTable.Columns.Add(new DataColumn(nameof(PAYEBatchItemsStaging.PayerId), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(PAYEBatchItemsStaging.IncomeTaxPerMonth), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(PAYEBatchItemsStaging.IncomeTaxPerMonthValue), typeof(decimal)));
                dataTable.Columns.Add(new DataColumn(nameof(PAYEBatchItemsStaging.Month), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(PAYEBatchItemsStaging.MonthValue), typeof(int)));
                dataTable.Columns.Add(new DataColumn(nameof(PAYEBatchItemsStaging.Year), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(PAYEBatchItemsStaging.YearValue), typeof(int)));
                dataTable.Columns.Add(new DataColumn(nameof(PAYEBatchItemsStaging.PAYEBatchRecordStaging) + "_Id", typeof(Int64)));
                dataTable.Columns.Add(new DataColumn(nameof(PAYEBatchItemsStaging.HasErrors), typeof(bool)));
                dataTable.Columns.Add(new DataColumn(nameof(PAYEBatchItemsStaging.ErrorMessages), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(PAYEBatchItemsStaging.AssessmentDate), typeof(DateTime)));
                dataTable.Columns.Add(new DataColumn(nameof(PAYEBatchItemsStaging.CreatedAtUtc), typeof(DateTime)));
                dataTable.Columns.Add(new DataColumn(nameof(PAYEBatchItemsStaging.UpdatedAtUtc), typeof(DateTime)));


                var batchItemsExemptionStagingDt = new DataTable("Parkway_CBS_Core_" + typeof(PAYEBatchItemExemptionStaging).Name);
                batchItemsExemptionStagingDt.Columns.Add(new DataColumn(nameof(PAYEBatchItemExemptionStaging.PAYEBatchItemsStaging) + "_Id", typeof(long)));
                batchItemsExemptionStagingDt.Columns.Add(new DataColumn(nameof(PAYEBatchItemExemptionStaging.PAYEExemptionType) + "_Id", typeof(int)));
                batchItemsExemptionStagingDt.Columns.Add(new DataColumn(nameof(PAYEBatchItemExemptionStaging.PAYEBatchRecordStaging) + "_Id", typeof(long)));
                batchItemsExemptionStagingDt.Columns.Add(new DataColumn(nameof(PAYEBatchItemExemptionStaging.AmountStringValue), typeof(string)));
                batchItemsExemptionStagingDt.Columns.Add(new DataColumn(nameof(PAYEBatchItemExemptionStaging.PAYEExemptionTypeName), typeof(string)));
                batchItemsExemptionStagingDt.Columns.Add(new DataColumn(nameof(PAYEBatchItemExemptionStaging.Amount), typeof(decimal)));
                batchItemsExemptionStagingDt.Columns.Add(new DataColumn(nameof(PAYEBatchItemExemptionStaging.SerialNumber), typeof(int)));
                batchItemsExemptionStagingDt.Columns.Add(new DataColumn(nameof(PAYEBatchItemExemptionStaging.CreatedAtUtc), typeof(DateTime)));
                batchItemsExemptionStagingDt.Columns.Add(new DataColumn(nameof(PAYEBatchItemExemptionStaging.UpdatedAtUtc), typeof(DateTime)));


                int serialNo = 1;

                while (stopper < pages)
                {
                    payeItems.Skip(skip).Take(chunkSize).ToList().ForEach(x =>
                    {
                        #region BatchItemsStagingDatatable
                        DataRow row = dataTable.NewRow();
                        row[nameof(PAYEBatchItemsStaging.HasErrors)] = false;
                        row[nameof(PAYEBatchItemsStaging.IncomeTaxPerMonth)] = DBNull.Value;
                        row[nameof(PAYEBatchItemsStaging.IncomeTaxPerMonthValue)] = DBNull.Value;
                        row[nameof(PAYEBatchItemsStaging.PayerId)] = x.PayerId;
                        row[nameof(PAYEBatchItemsStaging.Year)] = x.Year;
                        row[nameof(PAYEBatchItemsStaging.YearValue)] = DBNull.Value;
                        row[nameof(PAYEBatchItemsStaging.Month)] = x.Month;
                        row[nameof(PAYEBatchItemsStaging.MonthValue)] = DBNull.Value;
                        row[nameof(PAYEBatchItemsStaging.PAYEBatchRecordStaging) + "_Id"] = batchStagingId;
                        row[nameof(PAYEBatchItemsStaging.Exemptions)] = x.Exemptions;
                        row[nameof(PAYEBatchItemsStaging.ExemptionsValue)] = DBNull.Value;
                        row[nameof(PAYEBatchItemsStaging.GrossAnnual)] = x.GrossAnnualEarning;
                        row[nameof(PAYEBatchItemsStaging.GrossAnnualValue)] = DBNull.Value;
                        row[nameof(PAYEBatchItemsStaging.ErrorMessages)] = DBNull.Value;
                        row[nameof(PAYEBatchItemsStaging.AssessmentDate)] = DBNull.Value;
                        row[nameof(PAYEBatchItemsStaging.CreatedAtUtc)] = DateTime.Now.ToLocalTime();
                        row[nameof(PAYEBatchItemsStaging.UpdatedAtUtc)] = DateTime.Now.ToLocalTime();
                        dataTable.Rows.Add(row);
                        #endregion

                        #region PayeExemptionDatatable
                        x.PAYEExemptionTypes.ForEach(i =>
                        {
                            DataRow rowExemption = batchItemsExemptionStagingDt.NewRow();

                            rowExemption[nameof(PAYEBatchItemExemptionStaging.Amount)] = decimal.TryParse(i.Amount, out decimal amount) ? amount : 0;
                            rowExemption[nameof(PAYEBatchItemExemptionStaging.PAYEBatchRecordStaging) + "_Id"] = batchStagingId;
                            rowExemption[nameof(PAYEBatchItemExemptionStaging.PAYEExemptionType) + "_Id"] = i.Id;
                            rowExemption[nameof(PAYEBatchItemExemptionStaging.PAYEExemptionTypeName)] = i.Name;
                            rowExemption[nameof(PAYEBatchItemExemptionStaging.PAYEBatchItemsStaging) + "_Id"] = DBNull.Value;
                            rowExemption[nameof(PAYEBatchItemExemptionStaging.AmountStringValue)] = i.Amount;
                            rowExemption[nameof(PAYEBatchItemExemptionStaging.SerialNumber)] = serialNo;
                            rowExemption[nameof(PAYEBatchItemExemptionStaging.CreatedAtUtc)] = DateTime.Now.ToLocalTime();
                            rowExemption[nameof(PAYEBatchItemExemptionStaging.UpdatedAtUtc)] = DateTime.Now.ToLocalTime();
                            batchItemsExemptionStagingDt.Rows.Add(rowExemption);

                        });
                        #endregion

                        serialNo++;
                    });
                    Logger.Information(string.Format("Insertion for PAYE assessment batch items has started skip {0}", skip));

                    if (!SaveBundle(dataTable, "Parkway_CBS_Core_" + typeof(PAYEBatchItemsStaging).Name))
                    { throw new Exception("Error saving excel file details for batch Id " + batchStagingId); }

                    SaveBundleExemptionRecords(batchItemsExemptionStagingDt, batchStagingId);

                    skip += chunkSize;
                    stopper++;
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message); throw;
            }
        }

        public void SaveRecords(List<PayeeAssessmentLineRecordModel> payees, long batchStagingId)
        {
            Logger.Information("Saving PAYE records for batch id " + batchStagingId);
            try
            {
                using (var session = _transactionManager.GetSession().SessionFactory.OpenStatelessSession())
                using (var tranx = session.BeginTransaction())
                {
                    session.SetBatchSize(payees.Count());
                    try
                    {
                        foreach (PayeeAssessmentLineRecordModel item in payees)
                        {
                            PAYEBatchItemsStaging newRecord = new PAYEBatchItemsStaging
                            {
                                HasErrors = item.HasError,
                                IncomeTaxPerMonth = item.PayeeBreakDown.TaxStringValue,
                                IncomeTaxPerMonthValue = item.PayeeBreakDown.Tax,
                                PayerId = item.TaxPayerId.Value,
                                Year = item.Year.StringValue,
                                YearValue = item.Year.Value,
                                Month = item.Month.StringValue,
                                MonthValue = item.Month.Value,
                                PAYEBatchRecordStaging = new PAYEBatchRecordStaging { Id = batchStagingId },
                                Exemptions = item.Exemptions.StringValue,
                                ExemptionsValue = item.Exemptions.Value,
                                GrossAnnual = item.GrossAnnualEarnings.StringValue,
                                GrossAnnualValue = item.GrossAnnualEarnings.Value,
                                ErrorMessages = item.ErrorMessages,
                                AssessmentDate = item.AssessmentDate,
                            };
                            session.Insert(newRecord);
                        }
                        tranx.Commit();
                    }
                    catch (Exception exception)
                    {
                        Logger.Error(exception, exception.Message);
                        tranx.Rollback();
                        throw new Exception();
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

        private void SaveBundleExemptionRecords(DataTable exemptionItemsDt, long batchStagingId)
        {
            Logger.Information("Saving PAYE assessment exemption records for batch id " + batchStagingId);

            if (!SaveBundle(exemptionItemsDt, "Parkway_CBS_Core_" + typeof(PAYEBatchItemExemptionStaging).Name))
            { throw new Exception("Error saving PAYE assessment exemption records for batch id " + batchStagingId); }
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
                var query = _transactionManager.GetSession().CreateSQLQuery(queryText);
                query.SetParameter("batch_Id", batchId);
                query.SetParameter("bolVal", false);

                query.ExecuteUpdate();
            }
            catch (Exception)
            { throw; }
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
                var queryText = $"UPDATE its SET its.HasErrors = :bolVal, its.ErrorMessages = :errorMessage FROM Parkway_CBS_Core_PAYEBatchItemsStaging its WHERE its.PAYEBatchRecordStaging_Id = :batch_Id AND its.HasErrors = :falseBolVal AND its.TaxEntity_Id IS NULL";
                var query = _transactionManager.GetSession().CreateSQLQuery(queryText);
                query.SetParameter("batch_Id", batchId);
                query.SetParameter("falseBolVal", false);
                query.SetParameter("bolVal", true);
                query.SetParameter("errorMessage", ErrorLang.couldnotfinduserwithpayerid().ToString());

                query.ExecuteUpdate();
            }
            catch (Exception)
            { throw; }
        }

        /// <summary>
        /// Get the list of batch items
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns>IEnumerable{PayeeReturnModelVM}</returns>
        public IEnumerable<PayeeReturnModelVM> GetListOfPayes(long batchId, long taxEntityId, int skip, int take)
        {
            return _transactionManager.GetSession().Query<PAYEBatchItemsStaging>()
                 .Where(itm => ((itm.PAYEBatchRecordStaging == new PAYEBatchRecordStaging { Id = batchId }) && (itm.PAYEBatchRecordStaging.TaxEntity == new TaxEntity { Id = taxEntityId })))
                 .Skip(skip)
                 .Take(take)
                 .Select(py => new PayeeReturnModelVM
                 {
                     Exemptions = py.Exemptions,
                     GrossAnnual = py.GrossAnnual,
                     Month = py.Month,
                     TaxableIncome = py.IncomeTaxPerMonth,
                     PayerId = py.PayerId,
                     Year = py.Year,
                     HasError = py.HasErrors,
                     ErrorMessage = py.ErrorMessages,
                     PayeeName = py.TaxEntity != null ? py.TaxEntity.Recipient : string.Empty
                 }).ToFuture();
        }

        /// <summary>
        /// Get report aggregate
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns>IEnumerable{FileUploadReport}</returns>
        public IEnumerable<FileUploadReport> GetReportAggregate(long batchId, long taxEntityId)
        {
            return _transactionManager.GetSession()
                                  .CreateCriteria<PAYEBatchItemsStaging>(typeof(PAYEBatchItemsStaging).Name)
                                  .CreateAlias("PAYEBatchItemsStaging.PAYEBatchRecordStaging", "PAYEBatchRecordStaging")
                                  .Add(Restrictions.Eq("PAYEBatchRecordStaging.Id", batchId))
                                  .Add(Restrictions.Eq("PAYEBatchRecordStaging.TaxEntity.Id", taxEntityId))
                                  .Add(Restrictions.Eq("PAYEBatchItemsStaging.HasErrors", false))
                                  .SetProjection(
                                  Projections.ProjectionList()
                                      .Add(Projections.Sum<PAYEBatchItemsStaging>(x => (x.IncomeTaxPerMonthValue)), nameof(FileUploadReport.TotalAmountToBePaid))
                                      .Add(Projections.Count<PAYEBatchItemsStaging>(x => x.Id), nameof(FileUploadReport.NumberOfValidRecords))
                              ).SetResultTransformer(Transformers.AliasToBean<FileUploadReport>()).Future<FileUploadReport>();
        }

        public bool BatchItemStagingExist(string payerId, long payeBatchRecordStagingId)
        {
            return _transactionManager.GetSession().Query<PAYEBatchItemsStaging>().Count(l => l.PayerId == payerId && l.PAYEBatchRecordStaging.Id == payeBatchRecordStagingId) > 0;
        }

        /// <summary>
        /// Returns Only the PAYEBatchItemsStaging Id
        /// </summary>
        /// <param name="lambda"></param>
        /// <returns>The Id of the PAYEBatchItemsStaging</returns>
        /// <exception cref="NoRecordFoundException"> Thrown when no record is found matching the query</exception>
        public long GetPAYEBatchItemsStagingId(Expression<Func<PAYEBatchItemsStaging, bool>> lambda)
        {
            try
            {
                return _transactionManager.GetSession().Query<PAYEBatchItemsStaging>().Where(lambda)
                        .Select(txp => txp.Id).Single();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw new NoRecordFoundException();
            }
        }

        /// <summary>
        /// Get report aggregate
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns>IEnumerable{FileUploadReport}</returns>
        public IEnumerable<int> GetCount(long batchId)
        {
            return _transactionManager.GetSession()
                                  .CreateCriteria<PAYEBatchItemsStaging>(typeof(PAYEBatchItemsStaging).Name)
                                  .CreateAlias("PAYEBatchItemsStaging.PAYEBatchRecordStaging", "PAYEBatchRecordStaging")
                                  .Add(Restrictions.Eq("PAYEBatchRecordStaging.Id", batchId))
                                  .SetProjection(
                                  Projections.ProjectionList()
                                      .Add(Projections.Count<PAYEBatchItemsStaging>(x => (x.Id)))
                              ).Future<int>();
        }
    }
}