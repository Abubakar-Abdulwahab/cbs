using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.Transform;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Core.Services
{
    public class TaxPayerEnumerationItemsManager : BaseManager<TaxPayerEnumerationItems>, ITaxPayerEnumerationItemsManager<TaxPayerEnumerationItems>
    {
        private readonly IRepository<TaxPayerEnumerationItems> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;
        public ILogger Logger { get; set; }
        public TaxPayerEnumerationItemsManager(IRepository<TaxPayerEnumerationItems> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _user = user;
            _transactionManager = orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
        }

        /// <summary>
        /// Saves enumeration line items for enumeration batch with specified id.
        /// </summary>
        /// <param name="items">List<TaxPayerEnumerationLine></param>
        /// <param name="enumerationBatchId"></param>
        public void SaveRecords(IEnumerable<dynamic> items, long enumerationBatchId)
        {
            Logger.Information("Saving Tax Payer Enumeration records for batch id " + enumerationBatchId);
            try
            {
                using (var session = _transactionManager.GetSession().SessionFactory.OpenStatelessSession())
                using (var tranx = session.BeginTransaction())
                {
                    session.SetBatchSize(items.Count());
                    try
                    {
                        foreach (dynamic item in items)
                        {
                            TaxPayerEnumerationItems newRecord = new TaxPayerEnumerationItems
                            {
                                HasErrors = item.HasError,
                                TaxPayerName = item.Name,
                                TIN = item.TIN,
                                Address = item.Address,
                                PhoneNumber = item.PhoneNumber,
                                Email = item.Email,
                                LGA = item.LGA,
                                TaxPayerEnumeration = new TaxPayerEnumeration { Id = enumerationBatchId },
                                ErrorMessages = item.ErrorMessages,
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

        /// <summary>
        /// Returns enumeration line items for enumeration batch with specified id for the logged in or selected tax entity
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="taxEntityId"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public IEnumerable<TaxPayerEnumerationLine> GetLineItems(long batchId, long taxEntityId, int skip, int take)
        {
            try
            {
                return _transactionManager.GetSession().Query<TaxPayerEnumerationItems>()
                        .Where(x => x.TaxPayerEnumeration == new TaxPayerEnumeration { Id = batchId } && x.TaxPayerEnumeration.Employer == new TaxEntity { Id = taxEntityId })
                        .Skip(skip)
                        .Take(take)
                        .Select(x => new TaxPayerEnumerationLine
                        {
                            Name = x.TaxPayerName,
                            PhoneNumber = x.PhoneNumber,
                            Email = x.Email,
                            TIN = (string.IsNullOrEmpty(x.TIN)) ? "" : x.TIN,
                            LGA = x.LGA,
                            Address = x.Address,
                            HasError = x.HasErrors,
                            ErrorMessages = x.ErrorMessages
                        }).ToFuture();
            }catch(Exception exception)
            {
                Logger.Error(exception, $"Exception when trying to fetch enumeration line items for enumeration batch with id {batchId}. Exception message -- {exception.Message}");
                throw;
            }
        }

        /// <summary>
        /// Get report aggregate
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns>IEnumerable{FileUploadReport}</returns>
        public IEnumerable<FileUploadReport> GetReportAggregate(long batchId, long taxEntityId)
        {
            return _transactionManager.GetSession()
                                  .CreateCriteria<TaxPayerEnumerationItems>(typeof(TaxPayerEnumerationItems).Name)
                                  .CreateAlias("TaxPayerEnumerationItems.TaxPayerEnumeration", "TaxPayerEnumeration")
                                  .Add(Restrictions.Eq("TaxPayerEnumeration.Id", batchId))
                                  .Add(Restrictions.Eq("TaxPayerEnumeration.Employer.Id", taxEntityId))
                                  .Add(Restrictions.Eq("TaxPayerEnumerationItems.HasErrors", false))
                                  .SetProjection(
                                  Projections.ProjectionList()
                                      .Add(Projections.Count<TaxPayerEnumerationItems>(x => x.Id), nameof(FileUploadReport.NumberOfValidRecords))
                              ).SetResultTransformer(Transformers.AliasToBean<FileUploadReport>()).Future<FileUploadReport>();
        }

        /// <summary>
        /// Get report size
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns>IEnumerable{FileUploadReport}</returns>
        public IEnumerable<int> GetCount(long batchId)
        {
            return _transactionManager.GetSession()
                                  .CreateCriteria<TaxPayerEnumerationItems>(typeof(TaxPayerEnumerationItems).Name)
                                  .CreateAlias("TaxPayerEnumerationItems.TaxPayerEnumeration", "TaxPayerEnumeration")
                                  .Add(Restrictions.Eq("TaxPayerEnumeration.Id", batchId))
                                  .SetProjection(
                                  Projections.ProjectionList()
                                      .Add(Projections.Count<TaxPayerEnumerationItems>(x => (x.Id)))
                              ).Future<int>();
        }
    }
}