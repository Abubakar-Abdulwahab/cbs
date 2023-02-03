using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Core.Services
{
    public class TaxPayerEnumerationItemsStagingManager : BaseManager<TaxPayerEnumerationItemsStaging>, ITaxPayerEnumerationItemsStagingManager<TaxPayerEnumerationItemsStaging>
    {
        private readonly IRepository<TaxPayerEnumerationItemsStaging> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;
        public ILogger Logger { get; set; }
        public TaxPayerEnumerationItemsStagingManager(IRepository<TaxPayerEnumerationItemsStaging> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
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
                            TaxPayerEnumerationItemsStaging newRecord = new TaxPayerEnumerationItemsStaging
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
    }
}