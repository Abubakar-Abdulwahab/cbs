using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Services
{
    public class ReferenceDataBatchManager : BaseManager<ReferenceDataBatch>, IReferenceDataBatchManager<ReferenceDataBatch>
    {
        private readonly IRepository<ReferenceDataBatch> _repository;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;


        public ReferenceDataBatchManager(IRepository<ReferenceDataBatch> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _orchardServices = orchardServices;
            _transactionManager = orchardServices.TransactionManager;
            _repository = repository;
            Logger = NullLogger.Instance;
        }

        public ReferenceDataBatch GetBatch(Int64 batchId)
        {
            return _transactionManager.GetSession().QueryOver<ReferenceDataBatch>().Where(x => x.Id == batchId).SingleOrDefault();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="generalBatchRef"></param>
        /// <returns></returns>
        public ReferenceDataBatch GetBatchDetails(Int64 generalBatchId)
        {
            return _transactionManager.GetSession().QueryOver<ReferenceDataBatch>().Where(x => x.GeneralBatchReference == new GeneralBatchReference { Id = generalBatchId }).SingleOrDefault();
        }
    }
}