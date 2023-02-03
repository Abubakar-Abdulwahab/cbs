using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Users.Models;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;


namespace Parkway.CBS.Core.Services
{
    public class FormControlRevenueHeadValueManager : BaseManager<FormControlRevenueHeadValue>, IFormControlRevenueHeadValueManager<FormControlRevenueHeadValue>
    {
        private readonly IRepository<FormControlRevenueHeadValue> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public FormControlRevenueHeadValueManager(IRepository<FormControlRevenueHeadValue> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _transactionManager = orchardServices.TransactionManager;
        }

        /// <summary>
        /// Get the revenue head form control values for a particular invoice
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        public List<FormControlRevenueHeadValueVM> GetRevenueHeadFormControlValues(long invoiceId)
        {
            return _transactionManager.GetSession().Query<FormControlRevenueHeadValue>()
           .Where(x => x.Invoice == new Invoice { Id = invoiceId })
           .Select(f => new FormControlRevenueHeadValueVM
           {
               Name = f.FormControlRevenueHead.Form.Name,
               TechnicalName = f.FormControlRevenueHead.Form.TechnicalName,
               Value = f.Value
           }).ToList();
        }
    }
}