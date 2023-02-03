using Orchard;
using Orchard.Data;
using Orchard.Users.Models;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using System.Collections.Generic;
using NHibernate.Linq;
using System.Linq;
using Orchard.Logging;

namespace Parkway.CBS.Core.Services
{
    public class CountryManager : BaseManager<Country>, ICountryManager<Country>
    {
        private readonly IRepository<Country> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;


        public CountryManager(IRepository<Country> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _user = user;
            _transactionManager = orchardServices.TransactionManager;
        }


        /// <summary>
        /// Gets all active countries
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CountryVM> GetCountries()
        {
            try
            {
                return _transactionManager.GetSession().Query<Country>().Where(x => x.IsActive).Select(x => new CountryVM
                {
                    Id = x.Id,
                    Name = x.Name,
                    Code = x.Code
                }).ToFuture();
            }catch(System.Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
            
        }

        /// <summary>
        /// Gets country with specified id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CountryVM GetCountry(int id)
        {
            try
            {
                return _transactionManager.GetSession().Query<Country>().Where(x => x.Id == id).Select(x => new CountryVM { Id = x.Id, Name = x.Name }).SingleOrDefault();
            }catch(System.Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }
    }
}