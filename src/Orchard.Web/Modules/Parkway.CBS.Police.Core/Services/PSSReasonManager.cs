using Orchard;
using Orchard.Data;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Core.Services
{
    public class PSSReasonManager : BaseManager<PSSReason>, IPSSReasonManager<PSSReason>
    {
        private readonly IRepository<PSSReason> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public PSSReasonManager(IRepository<PSSReason> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = orchardServices.TransactionManager;
        }


        public ICollection<PSSReasonVM> GetReasonsVM()
        {
            return GetCollection()?.Select(x => new PSSReasonVM { Id = x.Id, Name = x.Name }).ToList();
        }

        public PSSReasonVM GetReasonVM(int id)
        {
            return GetCollection()?.Where(x => x.Id == id).Select(x => new PSSReasonVM { Id = x.Id, Name = x.Name }).FirstOrDefault();
        }
    }
}