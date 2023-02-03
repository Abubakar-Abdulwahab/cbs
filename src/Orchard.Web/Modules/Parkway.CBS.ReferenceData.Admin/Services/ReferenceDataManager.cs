using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Payee.ReferenceDataImplementation;
using Parkway.CBS.ReferenceData.Admin.Services.Contracts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Parkway.CBS.ReferenceData.Admin.Services
{
    public class ReferenceDataManager : BaseManager<Core.Models.ReferenceDataRecords>, IReferenceDataManager<Core.Models.ReferenceDataRecords>
    {
        private readonly IRepository<Core.Models.ReferenceDataRecords> _repository;
        private readonly IOrchardServices _orchardServices;
        private readonly IRepository<UserPartRecord> _user;
        private readonly ITransactionManager _transactionManager;

        public ReferenceDataManager(IRepository<Core.Models.ReferenceDataRecords> repository, IRepository<UserPartRecord> user, IOrchardServices orchardService) : base(repository, user, orchardService)
        {
            _transactionManager = orchardService.TransactionManager;
            _repository = repository;
            Logger = NullLogger.Instance;
        }
    }
}