using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Core.Services
{
    public class PSSCharacterCertificateRequestTypeManager : BaseManager<PSSCharacterCertificateRequestType>, IPSSCharacterCertificateRequestTypeManager<PSSCharacterCertificateRequestType>
    {
        private readonly IRepository<PSSCharacterCertificateRequestType> _repository;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public PSSCharacterCertificateRequestTypeManager(IRepository<PSSCharacterCertificateRequestType> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = orchardServices.TransactionManager;
        }


        /// <summary>
        /// Gets all active character certificate request types
        /// </summary>
        /// <returns>IEnumerable<CharacterCertificateRequestTypeVM></returns>
        public IEnumerable<CharacterCertificateRequestTypeVM> GetRequestTypes()
        {
            return _transactionManager.GetSession().Query<PSSCharacterCertificateRequestType>().Where(x => x.IsActive).Select(x => new CharacterCertificateRequestTypeVM
            {
                Id = x.Id,
                Name = x.Name
            });
        }


        /// <summary>
        /// Gets character certificate request type with specified id
        /// </summary>
        /// <param name="requestTypeId"></param>
        /// <returns>CharacterCertificateRequestTypeVM</returns>
        public CharacterCertificateRequestTypeVM GetRequestType(int requestTypeId)
        {
            return _transactionManager.GetSession().Query<PSSCharacterCertificateRequestType>().Where(x => x.IsActive && x.Id == requestTypeId).Select(x => new CharacterCertificateRequestTypeVM
            {
                Id = x.Id,
                Name = x.Name
            }).SingleOrDefault();
        }
    }
}