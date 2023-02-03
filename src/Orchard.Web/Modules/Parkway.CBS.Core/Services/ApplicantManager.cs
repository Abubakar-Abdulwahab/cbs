using Orchard.Data;
using Orchard.Users.Models;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Orchard;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Services
{
    public class ApplicantManager : BaseManager<Applicant>, IApplicantManager<Applicant>
    {
        private readonly IRepository<Applicant> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public ApplicantManager(IRepository<Applicant> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _user = user;
            _transactionManager = orchardServices.TransactionManager;

        }
    }
}