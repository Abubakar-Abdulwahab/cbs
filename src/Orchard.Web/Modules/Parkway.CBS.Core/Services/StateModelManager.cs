using NHibernate.Criterion;
using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Entities.DTO;
using Parkway.CBS.FileUpload;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Core.Services
{
    public class StateModelManager : BaseManager<StateModel>, IStateModelManager<StateModel>
    {
        private readonly IRepository<StateModel> _stateModelRepository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public StateModelManager(IRepository<StateModel> stateModelRepository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(stateModelRepository, user, orchardServices)
        {
            _stateModelRepository = stateModelRepository;
            _orchardServices = orchardServices;
            _transactionManager = orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
        }

        /// <summary>
        /// Get the list of state and their corresponding LGAs
        /// </summary>
        /// <returns>IList{OSGOFState}</returns>
        /// <exception cref="Exception">Throws exception</exception>
        public dynamic GetStatesAndLGAsForOSGOF()
        {
            try
            {
                var session = _transactionManager.GetSession();
                return session.Query<StateModel>().Select(s => new OSGOFState { Name = s.Name, Id = s.Id.ToString(), ListOfLGAs = s.LGAs.Select(l => new OSGOFStateLGA { Id = l.Id.ToString(), Name = l.Name }).ToList() }).ToList();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

        /// <summary>
        /// Get the list of state and their corresponding LGAs
        /// </summary>
        /// <returns>IList{StatesAndLGAs}</returns>
        /// <exception cref="Exception">Throws exception</exception>
        public List<StatesAndLGAs> GetStatesAndLGAs()
        {
            try
            {
                var session = _transactionManager.GetSession();
                return session.Query<StateModel>().Select(s => new StatesAndLGAs { LGAs = s.LGAs.Select(l => new LGAModel { LGACode = l.CodeName, LGAId = l.Id, Name = l.Name }).ToList(), Name = s.Name, StateCode = s.ShortName, StateId = s.Id }).ToList();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        public List<StateModel> GetStates() {

            try
            {
                var session = _transactionManager.GetSession();
                return session.Query<StateModel>().OrderBy(x => x.Name).Select(s => new StateModel { LGAs = s.LGAs.Select(l => new LGA { Name = l.Name, Id = l.Id }).ToList(), Name = s.Name, ShortName = s.ShortName, Id = s.Id }).ToList();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        public List<StateModelVM> GetStateVMs()
        {
            try
            {
                var session = _transactionManager.GetSession();
                return session.Query<StateModel>().Select(s => new StateModelVM { LGAs = s.LGAs.Select(l => new LGAVM { Name = l.Name, Id = l.Id }).ToList(), Name = s.Name, Id = s.Id }).ToList();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

        public IEnumerable<StateModel> GetAllStates()
        {
            try
            {
                var session = _transactionManager.GetSession();
                return session.Query<StateModel>().Select(s => new StateModel { Name = s.Name, Id = s.Id }).ToFuture();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="stateId"></param>
        /// <returns>list of LGAs of the state specified by the state Id</returns>
        /// <exception cref="">an exception is thrown if something goes wrong</exception>
        public List<LGA> GetLgas(int stateId)
        {
            try
            {
                var session = _transactionManager.GetSession();
                return session.Query<LGA>().Where(l => l.State.Id == stateId).Select( n => new LGA { Id = n.Id, Name = n.Name } ).ToList();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        public List<LGA> ValidateLga(int lgaId)
        {
            try
            {
                var session = _transactionManager.GetSession();
                return session.Query<LGA>().Where( l => l.Id == lgaId).Select(n => new LGA { Id = n.Id, Name = n.Name }).ToList();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

        /// <summary>
        /// Get the state Id for this LGA Id
        /// </summary>
        /// <param name="lgaId"></param>
        /// <param name="stateId"></param>
        /// <returns>int</returns>
        public int CountStateIdForLGAId(int lgaId, int stateId)
        {
            return _transactionManager.GetSession().Query<LGA>().Count(l => (l.Id == lgaId && l.State.Id == stateId));
        }


        public IEnumerable<StateModel> GetState(int stateId)
        {
            var session = _transactionManager.GetSession();
            return session.CreateCriteria<StateModel>().Add(Restrictions.Eq("Id", stateId)).Future<StateModel>();
        }

        /// <summary>
        /// Get state detail using the short name
        /// </summary>
        /// <param name="stateShortName"></param>
        /// <returns>IEnumerable<StateModel></returns>
        public IEnumerable<StateModel> GetState(string stateShortName)
        {
            var session = _transactionManager.GetSession();
            return session.CreateCriteria<StateModel>().Add(Restrictions.Eq(nameof(StateModel.ShortName), stateShortName)).Future<StateModel>();
        }


    }
}