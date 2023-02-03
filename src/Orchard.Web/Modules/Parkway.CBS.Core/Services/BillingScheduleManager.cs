using NHibernate.Criterion;
using NHibernate.Transform;
using Orchard;
using Orchard.Data;
using Orchard.Users.Models;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Services
{
    public class BillingScheduleManager : BaseManager<BillingSchedule>, IBillingScheduleManager<BillingSchedule>
    {
        private readonly IRepository<BillingSchedule> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;


        public BillingScheduleManager(IRepository<BillingSchedule> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _user = user;
            _transactionManager = orchardServices.TransactionManager;
        }

        /// <summary>
        /// Get schedules belonging to the tax payers
        /// </summary>
        /// <param name="billing"></param>
        /// <param name="taxPayers"></param>
        /// <returns></returns>
        public IEnumerable<BillingSchedule> GetScheduleForTaxPayers(BillingModel billing, IEnumerable<TaxEntity> taxPayers)
        {
            var session = _transactionManager.GetSession();
            var listOfTaxPayerIds = taxPayers.Select(txp => txp.TaxPayerIdentificationNumber).ToArray();

            var dfd =  session.CreateCriteria<BillingSchedule>()
               .Add(Restrictions.In("TaxPayerIdentificationNumber", listOfTaxPayerIds)).List<BillingSchedule>();

            return session.CreateCriteria<BillingSchedule>()
               .Add(Restrictions.In("TaxPayerIdentificationNumber", listOfTaxPayerIds)).List<BillingSchedule>();
        }
    }
}