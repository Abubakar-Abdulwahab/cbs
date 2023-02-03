using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Parkway.CBS.ReferenceData.Admin.Services.Contracts;
using Orchard.Data;
using Orchard;
using Orchard.Users.Models;
using Orchard.Logging;
using Parkway.CBS.ReferenceData.Admin.ViewModels;
using NHibernate.Linq;

namespace Parkway.CBS.ReferenceData.Admin.Services
{
    public class NagisInvoiceSummaryManager : BaseManager<NagisOldInvoiceSummary>, INagisInvoiceSummaryManager<NagisOldInvoiceSummary>
    {
        private readonly IRepository<NagisOldInvoiceSummary> _repository;
        private readonly IOrchardServices _orchardServices;
        private readonly IRepository<UserPartRecord> _user;
        private readonly ITransactionManager _transactionManager;


        public NagisInvoiceSummaryManager(IRepository<NagisOldInvoiceSummary> repository, IRepository<UserPartRecord> user, IOrchardServices orchardService) : base(repository, user, orchardService)
        {
            _transactionManager = orchardService.TransactionManager;
            _repository = repository;
            Logger = NullLogger.Instance;
        }

        public NAGISInvoiceSummaryVM GetInvoiceSummaries(long NagisDataBatchId)
        {
            NAGISInvoiceSummaryVM model = new NAGISInvoiceSummaryVM();
            model.ReportRecords = _transactionManager.GetSession().Query<NagisOldInvoiceSummary>().Where(x=> x.NagisDataBatch == new NagisDataBatch { Id = NagisDataBatchId } ).Select(x =>
             new NAGISInvoiceSummaryCollection()
             {
                 NAGISInvoiceNumber = x.NagisInvoiceNumber,
                 InvoiceNumber = x.InvoiceNumber,
                 PayerId = x.TaxEntity.PayerId
             });

            return model;
        }
    }
}