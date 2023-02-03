using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Users.Models;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using System.Linq;

namespace Parkway.CBS.Core.Services
{
    public class BillingModelManager : BaseManager<BillingModel>, IBillingModelManager<BillingModel>
    {
        private readonly IRepository<BillingModel> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public BillingModelManager(IRepository<BillingModel> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _transactionManager = orchardServices.TransactionManager;
        }


        /// <summary>
        /// Get the direct assessment model
        /// </summary>
        /// <param name="billingId"></param>
        /// <returns>string</returns>
        public string GetDirectAssessmentModel(int billingId)
        {
            return _transactionManager.GetSession().Query<BillingModel>()
                            .Where(bill => (bill == new BillingModel { Id = billingId }))
                            .Select(bill => bill.DirectAssessmentModel).ToList().FirstOrDefault();
        }

    }
}