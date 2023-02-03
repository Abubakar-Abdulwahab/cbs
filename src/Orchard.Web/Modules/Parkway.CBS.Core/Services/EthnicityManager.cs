using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Core.Services
{
    public class EthnicityManager : BaseManager<Ethnicity>, IEthnicityManager<Ethnicity>
    {
        private readonly IRepository<Ethnicity> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;


        public EthnicityManager(IRepository<Ethnicity> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _user = user;
            _transactionManager = orchardServices.TransactionManager;
        }


        /// <summary>
        /// Gets all active ethnicities
        /// </summary>
        /// <returns></returns>
        public IEnumerable<EthnicityVM> GetEthnicities()
        {
            try
            {
                return _transactionManager.GetSession().Query<Ethnicity>().Where(x => x.IsActive).Select(x => new EthnicityVM
                {
                    Id = x.Id,
                    Name = x.Name,
                }).ToFuture();
            }
            catch (System.Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }

        }

        /// <summary>
        /// Get ethnicity with specified id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public EthnicityVM GetEthnicity(int id)
        {
            try
            {
                return _transactionManager.GetSession().Query<Ethnicity>().Where(x => x.Id == id).Select(x => new EthnicityVM
                {
                    Id = x.Id,
                    Name = x.Name,
                }).SingleOrDefault();
            }
            catch(System.Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }
    }
}