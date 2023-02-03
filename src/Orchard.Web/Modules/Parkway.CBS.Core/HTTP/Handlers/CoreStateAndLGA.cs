using Orchard;
using Orchard.Logging;
using Parkway.CBS.CacheProvider;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;

namespace Parkway.CBS.Core.HTTP.Handlers
{
    public class CoreStateAndLGA : ICoreStateAndLGA
    {
        private readonly Lazy<IStateModelManager<StateModel>> _stateRepo;
        private readonly Lazy<ILGAManager<LGA>> _lgaRepo;
        private readonly IOrchardServices _orchardServices;

        public ILogger Logger { get; set; }

        public CoreStateAndLGA(Lazy<IStateModelManager<StateModel>> stateRepo, Lazy<ILGAManager<LGA>> lgaRepo, IOrchardServices orchardServices)
        {
            _stateRepo = stateRepo;
            _lgaRepo = lgaRepo;
            Logger = NullLogger.Instance;
            _orchardServices = orchardServices;
        }


        /// <summary>
        /// get all the states and their LGAs
        /// </summary>
        /// <returns>List{StateModel}</returns>
        public List<StateModel> GetStates()
        {
            string tenant = _orchardServices.WorkContext.CurrentSite.SiteName;

            List<StateModel> result = ObjectCacheProvider.GetCachedObject<List<StateModel>>(tenant, $"{nameof(CachePrefix.States)}");

            if (result == null)
            {
                result = _stateRepo.Value.GetStates();
                if (result != null)
                {
                    ObjectCacheProvider.TryCache(tenant, $"{nameof(CachePrefix.States)}", result);
                }
            }

            return result;
        }


        /// <summary>
        /// get all the states and their LGAs as view models
        /// </summary>
        /// <returns></returns>
        public List<StateModelVM> GetStateVMs()
        {
            return _stateRepo.Value.GetStateVMs();
        }


        /// <summary>
        /// Get state Id for this LGA Id
        /// </summary>
        /// <param name="lgaId"></param>
        /// <param name="stateId"></param>
        /// <returns>int</returns>
        public int GetStateIdForLGA(int lgaId, int stateId)
        {
            return _stateRepo.Value.CountStateIdForLGAId(lgaId, stateId);
        }


        /// <summary>
        /// Checks if state with specified id exists
        /// </summary>
        /// <param name="stateId"></param>
        /// <returns></returns>
        public bool ValidateState(int stateId)
        {
            return _stateRepo.Value.Count(x => x.Id == stateId) > 0;
        }


        /// <summary>
        /// Get state with specified id
        /// </summary>
        /// <param name="stateId"></param>
        /// <returns></returns>
        public IEnumerable<StateModel> GetState(int stateId)
        {
            return _stateRepo.Value.GetState(stateId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stateId"></param>
        /// <returns>list of LGAs of the state specified by the state Id</returns>
        public List<LGA> GetLgas(int stateId)
        {
            return _stateRepo.Value.GetLgas(stateId);
        }

        /// <summary>
        /// Get LGA with specified id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public LGAVM GetLGAWithId(int id)
        {
            return _lgaRepo.Value.GetLGAWithId(id);
        }
    }
}