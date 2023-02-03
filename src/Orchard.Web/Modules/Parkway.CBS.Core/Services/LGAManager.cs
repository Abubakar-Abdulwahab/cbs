using NHibernate.Criterion;
using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Users.Models;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Services
{
    public class LGAManager : BaseManager<LGA>, ILGAManager<LGA>
    {
        private readonly IRepository<LGA> _lgaRepository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public LGAManager(IRepository<LGA> lgaRepository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(lgaRepository, user, orchardServices)
        {
            _lgaRepository = lgaRepository;
            _orchardServices = orchardServices;
            _transactionManager = orchardServices.TransactionManager;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lgaId"></param>
        /// <returns></returns>
        public IEnumerable<LGA> GetLGA(int lgaId)
        {
            var session = _transactionManager.GetSession();
            return session.CreateCriteria<LGA>().Add(Restrictions.Eq("Id", lgaId)).Future<LGA>();
        }

        /// <summary>
        /// Get LGA detail using the lga code name
        /// </summary>
        /// <param name="lgaCodeName"></param>
        /// <returns>IEnumerable<LGA></returns>
        public IEnumerable<LGAVM> GetLGA(string lgaCodeName)
        {
            return _transactionManager.GetSession().Query<LGA>().Where(x => x.CodeName == lgaCodeName).Select(x=> new LGAVM { Id = x.Id, StateId = x.State.Id, StateShortName = x.State.ShortName });
        }

        /// <summary>
        /// Get LGAs
        /// </summary>
        /// <returns>List<LGAVM></returns>
        public List<LGAVM> GetLGAs()
        {
            return _transactionManager.GetSession().Query<LGA>().Select(x => new LGAVM { Id = x.Id, StateId = x.State.Id }).ToList();
        }

        /// <summary>
        /// Get LGA with specified id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public LGAVM GetLGAWithId(int id)
        {
            return _transactionManager.GetSession().Query<LGA>().Where(x => x.Id == id).Select(x => new LGAVM { Id = x.Id, Name = x.Name, StateId = x.State.Id, StateName = x.State.Name }).SingleOrDefault();
        }

    }
}