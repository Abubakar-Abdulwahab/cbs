using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.OSGOF.Admin.Models;
using Parkway.CBS.OSGOF.Admin.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.OSGOF.Admin.Services
{
    public class CellSiteClientPaymentBatchManager : BaseManager<CellSiteClientPaymentBatch>, ICellSiteClientPaymentBatchManager<CellSiteClientPaymentBatch>
    {
        private readonly IRepository<CellSiteClientPaymentBatch> _repository;
        private readonly IOrchardServices _orchardServices;
        private readonly IRepository<UserPartRecord> _user;
        private readonly ITransactionManager _transactionManager;

        public CellSiteClientPaymentBatchManager(IRepository<CellSiteClientPaymentBatch> repository, IRepository<UserPartRecord> user, IOrchardServices orchardService) : base(repository, user, orchardService)
        {
            _transactionManager = orchardService.TransactionManager;
            _repository = repository;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// Get the batch record with this Id
        /// </summary>
        /// <param name="recordId"></param>
        /// <returns>OSGOFBatchRecord</returns>
        public CellSiteClientPaymentBatch GetRecord(long id)
        {
            return _transactionManager.GetSession().Get<CellSiteClientPaymentBatch>(id);
        }
    }
}