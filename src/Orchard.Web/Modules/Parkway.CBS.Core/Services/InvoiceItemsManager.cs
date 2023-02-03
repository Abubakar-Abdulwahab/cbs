using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;

namespace Parkway.CBS.Core.Services
{
    public class InvoiceItemsManager : BaseManager<InvoiceItems>, IInvoiceItemsManager<InvoiceItems>
    {
        private readonly IRepository<InvoiceItems> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public InvoiceItemsManager(IRepository<InvoiceItems> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _transactionManager = orchardServices.TransactionManager;
        }


        public bool SaveBundleUnCommit(List<InvoiceItemFormsAndPosition> collection)
        {
            //TODO: meth is ok, but for the collection if chunked,
            // you might want to commit after all the chunks have been saved successfully
            var session = _transactionManager.GetSession();
            try
            {
                foreach (var item in collection)
                {
                    session.Save(item.InvoiceItems);
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Could not save object "));
                _transactionManager.GetSession().Transaction.Rollback();
                return false;
            }

            return true;
        }
    }
}